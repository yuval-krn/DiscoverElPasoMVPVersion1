using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using GoShared;
using System.Xml;

namespace GoMap {

	public class GODropFeatures : MonoBehaviour {

		public GOMap goMap;
		public TextAsset XmlDataFile;
		private List<DataEntry> _dataEntries = new List<DataEntry>();

		public Material testLineMaterial;
		public Material testPolygonMaterial;
        public GOUVMappingStyle uvMappingStyle = GOUVMappingStyle.TopFitSidesRatio;
		

		// Use this for initialization
		IEnumerator Start () {

			//Wait for the location manager to have the world origin set.
			yield return StartCoroutine (goMap.locationManager.WaitForOriginSet ());


			Debug.Log("we've started up");
			//Drop a point on the map
			LoadXmlFile();
			Debug.Log("end load xml file");
			dropPins();
			Debug.Log("end drop pins");
			Debug.Log(_dataEntries[0].name);

			//Drop a line
			
			//dropTestLine();

			//Drop a polygon
			//dropTestPolygon();

		}

		void dropPins() {
			bool first = true;
			Color32 EPblue = new Color32(100, 198, 189, 255);
			Color32 EPPink = new Color32(234, 54, 149, 255);

			foreach (var i in _dataEntries)
            {
				//1) create game object (you can instantiate a prefab instead)
				GameObject aBigSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				aBigSphere.name = i.name;
				aBigSphere.transform.localScale = new Vector3(10, 10, 10);
				SphereCollider sphc = aBigSphere.GetComponent<SphereCollider>();
				//Destroy(sphc);
				//aBigSphere.AddComponent<BoxCollider>();
				//BoxCollider hotspotCollider = aBigSphere.GetComponent<BoxCollider>();
				//hotspotCollider.size = new Vector3(2.0f, 2.0f, 2.0f);
				aBigSphere.tag = "hotspot";

				//check if been visited before
				if (PlayerPrefs.GetInt(i.name) == 1)
				{
					aBigSphere.GetComponent<Renderer>().material.SetColor("_Color", EPblue); 
				}
				else
				{
					if (first)
					{
						aBigSphere.GetComponent<Renderer>().material.SetColor("_Color", EPPink); 
						first = false;
					}
					else aBigSphere.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
				}

				//2) make a Coordinate class with your desired latitude longitude
				Coordinates coordinates = new Coordinates(i.lat, i.lng);
				//3) call drop pin passing the coordinates and your gameobject
				goMap.dropPin(coordinates, aBigSphere);
				
			}
		}

		void dropTestLine() {

			//1) Create a list of coordinates that will represent the polyline
			List <Coordinates> polyline = new List<Coordinates> ();
			polyline.Add(new Coordinates (34.06920494189155f, -118.35830119337147f));  
			polyline.Add(new Coordinates (34.067858468016176f, -118.35831053505827f));

			//2) Set line width
			float width = 3;

			//3) Set the line height
			float height = 2;

			//4) Choose a material for the line (this time we link the material from the inspector)
			Material material = testLineMaterial;

			//5) call drop line
            goMap.dropLine(polyline,width,height,material,uvMappingStyle);
				
		}

		void dropTestPolygon() {

			//Drop polygon is very similar to the drop line example, just make sure the coordinates will form a closed shape. 

			//1) Create a list of coordinates that will represent the polygon
			List <Coordinates> shape = new List<Coordinates> ();
			shape.Add(new Coordinates (48.8744621276855,2.29504323005676)); 
			shape.Add(new Coordinates (48.8744010925293,2.29542183876038)); 
			shape.Add(new Coordinates (48.8747596740723,2.29568862915039 )); 
			shape.Add(new Coordinates (48.8748931884766,2.29534268379211)); 
			shape.Add(new Coordinates (48.8748245239258,2.29496765136719)); 

			//2) Set the line height
			float height = 20;

			//3) Choose a material for the line (this time we link the material from the inspector)
			Material material = testPolygonMaterial;

			//4) call drop line
            goMap.dropPolygon(shape,height,material,uvMappingStyle);

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
}

