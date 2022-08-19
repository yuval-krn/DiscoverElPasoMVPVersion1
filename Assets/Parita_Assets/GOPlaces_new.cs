//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using GoShared;

//using System.Linq;

//namespace GoMap
//{
//    public class GOPlaces_new : MonoBehaviour
//    {
//        public GOMap goMap;

//        public string googleAPIkey;

//        string nearbySearchUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?";

//        // Use this for initialization

//        void Awake()
//        {

//            //register to the GOMap event OnTileLoad
//			goMap.OnTileLoad.AddListener ((GOTile) => {
//				OnLoadTile (GOTile);
//			});

//        }

//        void OnLoadTile(GOTile tile)
//        {

//        }


//    }

//}