using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        private void Start()
        {
            print(Application.persistentDataPath);
        }
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                 buildIndex = (int)state["lastSceneBuildIndex"];
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }
        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            CreateSaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }
        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }
        private void CreateSaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                //Transform playerTransform = GetPlayerTransform();
                //byte[] buffer = SerializeVector(playerTransform.position);

                BinaryFormatter formater = new BinaryFormatter();
                formater.Serialize(stream, state);

                //stream.Write(buffer, 0, buffer.Length);
            }
        }
        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                //byte[] buffer = new byte[stream.Length];
                //stream.Read(buffer, 0, buffer.Length);
                //Transform playerTransform = GetPlayerTransform();

                BinaryFormatter formater = new BinaryFormatter();
                return (Dictionary<string, object>)formater.Deserialize(stream);

                //RestoreState(formater.Deserialize(stream));
                //SerializableVector position = (SerializableVector)formater.Deserialize(stream);
                //playerTransform.position = position.ToVector();
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            //Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.Capture();
            }
            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }
        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.Restore(state[id]);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }

        //private byte[] SerializeVector(Vector3 vector)
        //{
        //    byte[] vectorBytes = new byte[3 * 4];
        //    BitConverter.GetBytes(vector.x).CopyTo(vectorBytes, 0);
        //    BitConverter.GetBytes(vector.y).CopyTo(vectorBytes, 4);
        //    BitConverter.GetBytes(vector.z).CopyTo(vectorBytes, 8);
        //    return vectorBytes;
        //}
        //private Vector3 DeserializeVector(byte[] buffer)
        //{
        //    Vector3 result = new Vector3();
        //    result.x = BitConverter.ToSingle(buffer, 0);
        //    result.y = BitConverter.ToSingle(buffer, 4);
        //    result.z = BitConverter.ToSingle(buffer, 8);
        //    return result;
        //}
        //private Transform GetPlayerTransform()
        //{
        //    return GameObject.FindWithTag("Player").transform;
        //}

    }
}

