using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    public static void SaveLevel(GridBase grid)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/level.dat", FileMode.Create);

        GridData data = new GridData(grid);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public static int[] LoadLevel()
    {
        if (File.Exists(Application.persistentDataPath + "/level.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/level.dat", FileMode.Open);

            GridData data = bf.Deserialize(stream) as GridData;
            stream.Close();

            return data.data;
        }
        else
        {
            return null;
        }
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