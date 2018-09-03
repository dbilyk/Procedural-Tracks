using UnityEngine;
using System.Collections.Generic;
using System.IO;

class CityNames : MonoBehaviour
{
  //grab CityNames.json file
  string jsonPath;
  //this file is never altered, just read in order to reset in case user goes through all 2k maps (yea right...)
  string jsonResetPath;
  StreamWriter citiesWriter;

  string citiesString;

  private Wrapper<Entry> CityData;
  void Awake(){
    jsonPath = Application.streamingAssetsPath + "/data/CityNames.json";
    jsonResetPath  = Application.streamingAssetsPath + "/data/CityNames_All.json";
    CityData = GetCitiesFromJson(jsonPath);
    
  }

  void OnApplicationQuit(){
    UpdateCitiesJson();
  }

  void OnApplicationFocus(bool state){
    if(state == false){
      UpdateCitiesJson();
    }
  }


//this function is to be consumed by the UI in order to grab a random city name for tracks.
  public string GetRandomCity(){
    //reset the city data to original 2k entries before getting a city
    if(CityData.City.Count == 0){
      CityData = GetCitiesFromJson(jsonResetPath);
      UpdateCitiesJson();
    }

    int random = Random.Range(0,CityData.City.Count);
    string result = CityData.City[random].Name;
    CityData.City.RemoveAt(random);
    return result;
  }

  //simply writes the current state of CityData to json file.
  private void UpdateCitiesJson(){
    citiesString = JsonUtility.ToJson(CityData);
    citiesWriter = new StreamWriter(jsonPath,false);
    citiesWriter.Write(citiesString);
    citiesWriter.Dispose();
  }

  //given a path, returns Wrapper object that contains data at the given path
  private Wrapper<Entry> GetCitiesFromJson(string path){
    StreamReader citiesStream = new StreamReader(path);
    citiesString = citiesStream.ReadToEnd();
    citiesStream.Dispose();
    return JsonUtility.FromJson<Wrapper<Entry>>(citiesString);
  }

  //JSON classes
  [System.Serializable]
  private class Wrapper<T>
  {
    public List<T> City;
  }

  [System.Serializable]
  private class Entry
  {
    public string Name;
  }


}

