using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    public static void SaveLevel(GridBase grid, string levelName, bool customLevel=true)
    {
        string path = customLevel ? Application.persistentDataPath + "/" :
                                    Application.dataPath + "/PresetMaps/";
        path += levelName + ".dat";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        GridData data = new GridData(grid);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static int[] LoadLevel(string levelName, bool customLevel=true)
    {
        string path = customLevel ? Application.persistentDataPath + "/" :
                                    Application.dataPath + "/PresetMaps/";
        path += levelName + ".dat";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GridData data = bf.Deserialize(stream) as GridData;
            stream.Close();

            return data.data;
        }
        else
        {
            return null;
        }
    }

    public static List<string> GetSavedLevels()
    {
        List<string> levelNames = new List<string>();
        foreach (string path in new List<string>() { Application.persistentDataPath + "/",
                                                     Application.dataPath + "/PresetMaps/" })
        {
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    if (file.EndsWith(".dat"))
                    {
                        levelNames.Add(System.IO.Path.GetFileNameWithoutExtension(file));
                    }
                }
            }
        }
        return levelNames;
    }
}

[System.Serializable]
public class GridData
{
    public int[] data;

    public GridData(GridBase grid)
    {
        // data = new int[grid.sizeX, grid.sizeZ];
        data = new int[grid.sizeX * grid.sizeZ * 2 + 2];
        data[0] = grid.sizeX;
        data[1] = grid.sizeZ;

        for (int x = 0; x < grid.sizeX; x++)
        {
            for (int z = 0; z < grid.sizeZ; z++)
            {
                int index = (x * grid.sizeZ + z) * 2 + 2;
                // Prefab ID
                data[index] = grid.grid[x, z].objId;

                if (data[index] != -1 && grid.grid[x, z].vis != null)
                {
                    Vector3 rot = grid.grid[x, z].vis.transform.rotation.eulerAngles;
                    data[index + 1] = (int)rot.y;
                }
                else
                {
                    data[index + 1] = -1;
                }
            }
        }
    }
}