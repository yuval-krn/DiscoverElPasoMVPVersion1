#if JSON_NET_EXISTS
using DA_Assets.FCU.Exceptions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

namespace DA_Assets.FCU
{
    class FCU_Mehtods
    {
        public static void Auth(FigmaConverterUnity fcu)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(WebClient.Authorize((token) =>
            {
                fcu.mainSettings.ApiKey = token;
                Console.WriteLine(Localization.AUTH_COMPLETE);
            }));
        }
        public static void DownloadProject(FigmaConverterUnity fcu)
        {
            fcu.downloadedPages.Clear();
            fcu.pagesForSelect.Clear();
            fcu.framesToDownload.Clear();
            fcu.downloadedFrames.Clear();

            Checkers.IsValidSettings();

            Console.WriteLine(Localization.STARTING_PROJECT_DOWNLOAD);
#if I2LOC_EXISTS
            if (fcu.mainSettings.UseI2Localization)
            {
                CanvasDrawer.InstantiateI2LocalizationSource();
            }
#endif

            EditorCoroutineUtility.StartCoroutineOwnerless(WebClient.GetProject(
            (fproject) =>
            {
                fcu.downloadedPages = fproject.Document.Children.ToList();

                fcu.pagesForSelect = new List<SelectableFObject>();

                foreach (FObject page in fcu.downloadedPages)
                {
                    page.FTag = FTag.Page;
                    fcu.pagesForSelect.Add(new SelectableFObject
                    {
                        Id = page.Id,
                        Name = page.Name,
                        Selected = false
                    });
                }

                if (fcu.pagesForSelect.Count() == 1)
                {
                    fcu.pagesForSelect[0].Selected = true;
                    GetFramesFromSelectedPage(fcu);
                }
                else
                {
                    fcu.getPageFramesButtonVisible = true;
                    Console.WriteLine(Localization.PROJECT_DOWNLOADED);
                }
            },
            (figmaError) =>
            {
                switch (figmaError.Status)
                {
                    case 404:
                        Console.Error(Localization.PROJECT_NOT_FOUND);
                        break;
                    default:
                        Console.Error(string.Format(Localization.UNKNOWN_ERROR, figmaError.Status, figmaError.Error, Constants.TG_LINK));
                        break;
                }
            }));
        }
        public static void GetFramesFromSelectedPage(FigmaConverterUnity fcu)
        {
            fcu.framesToDownload.Clear();
            fcu.downloadedFrames.Clear();

            SelectableFObject _selectedPage = fcu.pagesForSelect.FirstOrDefault(x => x.Selected == true);
            if (_selectedPage == null)
            {
                throw new NoSelectedPageException();
            }

            fcu.selectedPage = fcu.downloadedPages.FirstOrDefault(x => x.Id == _selectedPage.Id);

            fcu.framesToDownload = new List<SelectableFObject>();

            foreach (FObject frame in fcu.selectedPage.Children)
            {
                if (frame.Type == FTag.Frame.GetDescription().ToUpper())
                {
                    frame.FTag = FTag.Frame;
                    fcu.framesToDownload.Add(new SelectableFObject
                    {
                        Id = frame.Id,
                        Name = frame.Name,
                        Selected = true
                    });
                }
            }


            if (fcu.framesToDownload.Count() == 1)
            {
                fcu.framesToDownload[0].Selected = true;
                DownloadSelectedFrames(fcu);
            }
            else
            {
                if (fcu.framesToDownload.Count > 0)
                {
                    fcu.getPageFramesButtonVisible = false;
                    fcu.downloadFramesButtonVisible = true;
                    Console.WriteLine(string.Format(Localization.FRAMES_FINDED, fcu.framesToDownload.Count, fcu.selectedPage.Name));
                }
                else
                {
                    Console.Error(Localization.FRAMES_NOT_FINDED);
                }
            }
        }
        public static void DownloadSelectedFrames(FigmaConverterUnity fcu)
        {
            List<FObject> frames = new List<FObject>();
            List<SelectableFObject> selectedFrames = fcu.framesToDownload.Where(x => x.Selected == true).ToList();

            foreach (SelectableFObject frame in selectedFrames)
            {
                List<FObject> _frames = fcu.selectedPage.Children.Where(x => x.Id == frame.Id).ToList();
                frames.AddRange(_frames);
            }

            fcu.selectedPage.Children = frames;

            List<FObject> parsedFObjects = FigmaParser.GetChildrenOfPage(fcu.selectedPage);
            List<FObject> fobjectsWithRootFrames = FigmaParser.GetSetRootFrameForFObjects(parsedFObjects);
            List<FObject> fobjectsCheckedForMutualFObjects = FigmaParser.GetMutualFObjects(frames, parsedFObjects);

            EditorCoroutineUtility.StartCoroutineOwnerless(WebClient.GetImageLinksForFObjects(fobjectsCheckedForMutualFObjects, (linked) =>
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(WebClient.DownloadSpritesAsync(linked, (_downloadedFrames) =>
                {
                    fcu.downloadedFrames = _downloadedFrames;
                    fcu.downloadFramesButtonVisible = false;
                    fcu.getPageFramesButtonVisible = false;

                    DrawDownloadedFrames(fcu);
                }));
            }));
        }

        public static void DrawDownloadedFrames(FigmaConverterUnity fcu)
        {
            FCU_Prefs.ClearImportedFramesList();

            fcu.transform.localScale = Vector3.one;
            CanvasDrawer.DrawToCanvas(fcu.downloadedFrames);

            Console.Success(Localization.IMPORT_COMPLETE);
        }
    }
}
#endif