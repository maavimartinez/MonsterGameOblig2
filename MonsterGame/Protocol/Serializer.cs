﻿using System;

namespace Protocol
{

    public static class Serializer
    {

        public static string Serialize(object[] objectArray)
        {
            var serializedObjects = new string[objectArray.Length];
            for (var i = 0; i < objectArray.Length; i++)
            {
                serializedObjects[i] = objectArray[i] + "|";
            }
            return String.Join("",serializedObjects);
        }

        public static string[] DeSerialize(string serializedObj)
        {
            string trimmedObject = serializedObj.Trim('|');
            string[] objects = trimmedObject.Split('|');
            return objects;
        }

    }

}