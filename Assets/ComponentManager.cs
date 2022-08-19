using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Xml;

public class ComponentManager : MonoBehaviour
{
    public TextAsset XmlDataFile;
    private List<DataEntry> _dataEntries = new List<DataEntry>();

	[SerializeField] private GameObject redEll;
	[SerializeField] private GameObject yelEll;
	[SerializeField] private GameObject bluEll;
	private List<GameObject> ellipses = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
		LoadXmlFile();

		ellipses.Add(redEll);
		ellipses.Add(yelEll);
		ellipses.Add(bluEll);
		//call for-loop function that will create a circle for every data entry
		//then fill it with image and maybe dummy text below?
		placeEllipses();
    }

	void placeEllipses()
    {
		int counter = 0;

		foreach (var i in _dataEntries)
        {
			GameObject curEll = Instantiate(ellipses[counter % ellipses.Count]);
			curEll.name = i.name;
			
			curEll.transform.SetParent(transform);
			curEll.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

			counter++;
		}
    }

	void LoadXmlFile()
	{
		var xmlString = XmlDataFile.text;

		Debug.Log(xmlString);

		XmlDocument xmlDoc = new XmlDocument();

		try
		{
			xmlDoc.LoadXml(xmlString);
		}
		catch (XmlException e)
		{
			Debug.LogError("[ARLocation#WebMapLoader]: Failed to parse XML file: " + e.Message);
		}

		var root = xmlDoc.FirstChild;
		var nodes = root.ChildNodes;
		foreach (XmlNode node in nodes)
		{
			Debug.Log(node.InnerXml);
			Debug.Log(node["id"].InnerText);

			int id = int.Parse(node["id"].InnerText);
			double lat = double.Parse(node["lat"].InnerText, CultureInfo.InvariantCulture);
			double lng = double.Parse(node["lng"].InnerText, CultureInfo.InvariantCulture);
			double altitude = double.Parse(node["altitude"].InnerText, CultureInfo.InvariantCulture);
			//string altitudeMode = node["altitudeMode"].InnerText;
			string name = node["name"].InnerText;
			//string meshId = node["meshId"].InnerText;
			//float movementSmoothing = float.Parse(node["movementSmoothing"].InnerText, CultureInfo.InvariantCulture);
			//int maxNumberOfLocationUpdates = int.Parse(node["maxNumberOfLocationUpdates"].InnerText);
			//bool useMovingAverage = bool.Parse(node["useMovingAverage"].InnerText);
			//bool hideObjectUtilItIsPlaced = bool.Parse(node["hideObjectUtilItIsPlaced"].InnerText);

			DataEntry entry = new DataEntry()
			{
				id = id,
				lat = lat,
				lng = lng,
				//altitudeMode = altitudeMode,
				//altitude = altitude,
				name = name,
				//meshId = meshId,
				//movementSmoothing = movementSmoothing,
				//maxNumberOfLocationUpdates = maxNumberOfLocationUpdates,
				//useMovingAverage = useMovingAverage,
				//hideObjectUtilItIsPlaced = hideObjectUtilItIsPlaced
			};

			_dataEntries.Add(entry);

			Debug.Log($"{id}, {lat}, {lng}, {name}");
		}
	}

	public class DataEntry
	{
		public int id;
		public double lat;
		public double lng;
		//public double altitude;
		//public string altitudeMode;
		public string name;
		//public string meshId;
		//public float movementSmoothing;
		//public int maxNumberOfLocationUpdates;
		//public bool useMovingAverage;
		//ublic bool hideObjectUtilItIsPlaced;
		/*
		public AltitudeMode getAltitudeMode()
		{
			if (altitudeMode == "GroundRelative")
			{
				return AltitudeMode.GroundRelative;
			}
			else if (altitudeMode == "DeviceRelative")
			{
				return AltitudeMode.DeviceRelative;
			}
			else if (altitudeMode == "Absolute")
			{
				return AltitudeMode.Absolute;
			}
			else
			{
				return AltitudeMode.Ignore;
			}
		}
		*/
	}
}
