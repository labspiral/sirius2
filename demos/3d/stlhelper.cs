using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Demos
{
    /// <summary>
    /// 3 * Vertices and Normals
    /// </summary>
    public class Face
    {
        public Vector3 Normal1;
        public Vector3 Normal2;
        public Vector3 Normal3;
        public Vector3 V1;
        public Vector3 V2;
        public Vector3 V3;

        public Face()
        {
            Normal1 = new Vector3();
            Normal2 = new Vector3();
            Normal3 = new Vector3();
            V1 = new Vector3();
            V2 = new Vector3();
            V3 = new Vector3();
        }
    }

    /// <summary>
    /// StereoLithography helper</para>
    /// </summary>
    public class STLHelper
    {
        public string path;
        private enum FileType { Unknown, Binary, ASCII };
        public bool IsError { get; private set; }

        public STLHelper(string filePath = "")
        {
            path = filePath;
            IsError = false;
        }

        /// <summary>
        /// Read from file
        /// </summary>
        /// <returns>Array of face</returns>
        public Face[] ReadFile()
        {
            Face[] meshList = null;
            FileType stlFileType = GetFileType(path);
            switch(stlFileType)
            {
                case FileType.ASCII:
                   meshList = ReadASCIIFile(path);
                    break;
                case FileType.Binary:
                    meshList = ReadBinaryFile(path);
                    break;
            }
            return meshList;
        }

        /// <summary>
        /// Min 
        /// </summary>
        /// <param name="meshArray"></param>
        /// <returns></returns>
        public static Vector3 GetMinMeshPosition(Face[] meshArray)
        {
            Vector3 minVec = new Vector3();
            float[] minRefArray = new float[3];
            minRefArray[0] = meshArray.Min(j => j.V1.X);
            minRefArray[1] = meshArray.Min(j => j.V2.X);
            minRefArray[2] = meshArray.Min(j => j.V3.X);
            minVec.X = minRefArray.Min();
            minRefArray[0] = meshArray.Min(j => j.V1.Y);
            minRefArray[1] = meshArray.Min(j => j.V2.Y);
            minRefArray[2] = meshArray.Min(j => j.V3.Y);
            minVec.Y = minRefArray.Min();
            minRefArray[0] = meshArray.Min(j => j.V1.Z);
            minRefArray[1] = meshArray.Min(j => j.V2.Z);
            minRefArray[2] = meshArray.Min(j => j.V3.Z);
            minVec.Z = minRefArray.Min();
            return minVec;
        }

        /// <summary>
        /// Max
        /// </summary>
        /// <param name="meshArray"></param>
        /// <returns></returns>
        public static Vector3 GetMaxMeshPosition(Face[] meshArray)
        {
            Vector3 maxVec = new Vector3();
            float[] maxRefArray = new float[3];
            maxRefArray[0] = meshArray.Max(j => j.V1.X);
            maxRefArray[1] = meshArray.Max(j => j.V2.X);
            maxRefArray[2] = meshArray.Max(j => j.V3.X);
            maxVec.X = maxRefArray.Max();
            maxRefArray[0] = meshArray.Max(j => j.V1.Y);
            maxRefArray[1] = meshArray.Max(j => j.V2.Y);
            maxRefArray[2] = meshArray.Max(j => j.V3.Y);
            maxVec.Y = maxRefArray.Max();
            maxRefArray[0] = meshArray.Max(j => j.V1.Z);
            maxRefArray[1] = meshArray.Max(j => j.V2.Z);
            maxRefArray[2] = meshArray.Max(j => j.V3.Z);
            maxVec.Z = maxRefArray.Max();
            return maxVec;
        }

        /// <summary>
        /// STL file type (ASCII/Binary)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private FileType GetFileType(string filePath)
        {
            FileType stlFileType = FileType.Unknown;
            if (File.Exists(filePath))
            {
                int lineCount = 0;
                lineCount = File.ReadLines(filePath).Count(); // number of lines in the file
                string firstLine = File.ReadLines(filePath).First();
                string endLines = File.ReadLines(filePath).Skip(lineCount - 1).Take(1).First() +
                                  File.ReadLines(filePath).Skip(lineCount - 2).Take(1).First();
                if ((firstLine.IndexOf("solid") != -1) &
                    (endLines.IndexOf("endsolid") != -1))
                {
                    stlFileType = FileType.ASCII;
                }
                else
                {
                    stlFileType = FileType.Binary;
                }
            }
            else
            {
                stlFileType = FileType.Unknown;
            }
            return stlFileType;
        }

        /// <summary>
        /// Read STL file with binary format
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Face[] ReadBinaryFile(string filePath)
        {
            List<Face> meshList = new List<Face>();
            int numOfMesh = 0;
            int i = 0;
            int byteIndex = 0;
            byte[] fileBytes = File.ReadAllBytes(filePath);
            byte[] temp = new byte[4];

            /* 80 bytes title + 4 byte num of triangles + 50 bytes (1 of triangular mesh)  */
            if (fileBytes.Length > 120)
            {
                temp[0] = fileBytes[80];
                temp[1] = fileBytes[81];
                temp[2] = fileBytes[82];
                temp[3] = fileBytes[83];

                numOfMesh = System.BitConverter.ToInt32(temp, 0);
                byteIndex = 84;
                meshList.Capacity = numOfMesh;

                for (i = 0; i < numOfMesh; i++)
                {
                    Face newMesh = new Face();

                    /* this try-catch block will be reviewed */
                    try
                    {
                        /* face normal */
                        newMesh.Normal1.X = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;
                        newMesh.Normal1.Y = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;
                        newMesh.Normal1.Z = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;

                        /* normals of vertex 2 and 3 equals to vertex 1's normals */
                        newMesh.Normal2 = newMesh.Normal1;
                        newMesh.Normal3 = newMesh.Normal1;

                        /* vertex 1 */
                        newMesh.V1.X = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;
                        newMesh.V1.Y = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;
                        newMesh.V1.Z = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;

                        /* vertex 2 */
                        newMesh.V2.X = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;
                        newMesh.V2.Y = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;
                        newMesh.V2.Z = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;

                        /* vertex 3 */
                        newMesh.V3.X = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;
                        newMesh.V3.Y = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;
                        newMesh.V3.Z = System.BitConverter.ToSingle(new byte[] { fileBytes[byteIndex], fileBytes[byteIndex + 1], fileBytes[byteIndex + 2], fileBytes[byteIndex + 3] }, 0);
                        byteIndex += 4;

                        byteIndex += 2; // Attribute byte count
                    }
                    catch
                    {
                        IsError = true;
                        break;
                    }
                    meshList.Add(newMesh);
                }
            }
            else
            {
            }
            return meshList.ToArray();
        }

        /// <summary>
        /// Read STL file with ASCII format
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Face[] ReadASCIIFile(string filePath)
        {
            List<Face> meshList = new List<Face>();
            StreamReader txtReader = new StreamReader(filePath);
            string lineString;
            while (!txtReader.EndOfStream)
            {
                lineString = txtReader.ReadLine().Trim(); /* delete whitespace in front and tail of the string */
                string[] lineData = lineString.Split(' ');

                if (lineData[0] == "solid")
                {
                    while (lineData[0] != "endsolid")
                    {
                        lineString = txtReader.ReadLine().Trim(); // facetnormal
                        lineData = lineString.Split(' ');

                        if (lineData[0] == "endsolid") // check if we reach at the end of file
                        {
                            break;
                        }
                        Face newMesh = new Face(); // define new mesh object
                        /* this try-catch block will be reviewed */
                        try
                        {
                            // FaceNormal 
                            newMesh.Normal1.X = float.Parse(lineData[2]);
                            newMesh.Normal1.Y = float.Parse(lineData[3]);
                            newMesh.Normal1.Z = float.Parse(lineData[4]);

                            /* normals of vertex 2 and 3 equals to vertex 1's normals */
                            newMesh.Normal2 = newMesh.Normal1;
                            newMesh.Normal3 = newMesh.Normal1;

                            //----------------------------------------------------------------------
                            lineString = txtReader.ReadLine(); // Just skip the OuterLoop line
                            //----------------------------------------------------------------------

                            // Vertex1
                            lineString = txtReader.ReadLine().Trim();
                            /* reduce spaces until string has proper format for split */
                            while (lineString.IndexOf("  ") != -1) lineString = lineString.Replace("  ", " ");
                            lineData = lineString.Split(' ');

                            newMesh.V1.X = float.Parse(lineData[1]); // x1
                            newMesh.V1.Y = float.Parse(lineData[2]); // y1
                            newMesh.V1.Z = float.Parse(lineData[3]); // z1

                            // Vertex2
                            lineString = txtReader.ReadLine().Trim();
                            /* reduce spaces until string has proper format for split */
                            while (lineString.IndexOf("  ") != -1) lineString = lineString.Replace("  ", " ");
                            lineData = lineString.Split(' ');

                            newMesh.V2.X = float.Parse(lineData[1]); // x2
                            newMesh.V2.Y = float.Parse(lineData[2]); // y2
                            newMesh.V2.Z = float.Parse(lineData[3]); // z2

                            // Vertex3
                            lineString = txtReader.ReadLine().Trim();
                            /* reduce spaces until string has proper format for split */
                            while (lineString.IndexOf("  ") != -1) lineString = lineString.Replace("  ", " ");
                            lineData = lineString.Split(' ');

                            newMesh.V3.X = float.Parse(lineData[1]); // x3
                            newMesh.V3.Y = float.Parse(lineData[2]); // y3
                            newMesh.V3.Z = float.Parse(lineData[3]); // z3
                        }
                        catch
                        {
                            IsError = true;
                            break;
                        }

                        //----------------------------------------------------------------------
                        lineString = txtReader.ReadLine(); // Just skip the endloop
                        //----------------------------------------------------------------------
                        lineString = txtReader.ReadLine(); // Just skip the endfacet

                        meshList.Add(newMesh); // add mesh to meshList

                    } // while linedata[0]
                } // if solid
            } // while !endofstream

            return meshList.ToArray();
        }

        /// <summary>
        /// Face to array of vertices
        /// </summary>
        /// <param name="meshArray"></param>
        /// <returns></returns>
        public static float[] FacesToVertices(Face[] meshArray)
        {
            List<float> vertices = new List<float>(meshArray.Length * 9);

            for (int i = 0; i < meshArray.Length; i++)
            {
                /* vertex 1 */
                vertices.Add(meshArray[i].V1.X);
                vertices.Add(meshArray[i].V1.Y);
                vertices.Add(meshArray[i].V1.Z);
                /* vertex 2 */
                vertices.Add(meshArray[i].V2.X);
                vertices.Add(meshArray[i].V2.Y);
                vertices.Add(meshArray[i].V2.Z);
                /* vertex 3 */
                vertices.Add(meshArray[i].V3.X);
                vertices.Add(meshArray[i].V3.Y);
                vertices.Add(meshArray[i].V3.Z);
            }

            return vertices.ToArray();
        }

        /// <summary>
        /// Fact to array of normals
        /// </summary>
        /// <param name="meshArray"></param>
        /// <returns></returns>
        public static float[] FacesToNormals(Face[] meshArray)
        {
            List<float> normals = new List<float>(meshArray.Length * 9);

            for (int i = 0; i < meshArray.Length; i++)
            {
                /* normal 1 */
                normals.Add(meshArray[i].Normal1.X);
                normals.Add(meshArray[i].Normal1.Y);
                normals.Add(meshArray[i].Normal1.Z);
                /* normal 2 */
                normals.Add(meshArray[i].Normal2.X);
                normals.Add(meshArray[i].Normal2.Y);
                normals.Add(meshArray[i].Normal2.Z);
                /* normal 3 */
                normals.Add(meshArray[i].Normal3.X);
                normals.Add(meshArray[i].Normal3.Y);
                normals.Add(meshArray[i].Normal3.Z);
            }

            return normals.ToArray();
        }
    }
}
