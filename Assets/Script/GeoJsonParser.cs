using System;
using System.Collections.Generic;

[Serializable]
public class Geometry
{
    public string type;
    public List<List<double>> coordinates; // [lng, lat] untuk LineString
}

[Serializable]
public class Properties
{
    public int id;
    public string ShltrRoute; // Atau field lain dari GeoJSON Anda
}

[Serializable]
public class Feature
{
    public string type;
    public Properties properties;
    public Geometry geometry;
}

[Serializable]
public class FeatureCollection
{
    public string type;
    public string name;
    public List<Feature> features;
}
