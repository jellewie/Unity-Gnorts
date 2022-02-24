using UnityEngine;

namespace PublicCode
{
    public static class RemoveElementArray{
         public static Collider[] removeAtIndexColliderArray(Collider[] inputArray, int index){
            Collider[] outputArray = new Collider[inputArray.Length - 1];
            for (int i = 0; i < index; i++){
                outputArray[i] = inputArray[i];
            }
            int writeLoc = index;
            int readLoc = index + 1;
            for (writeLoc = index; writeLoc < outputArray.Length; writeLoc++){
                outputArray[writeLoc] = inputArray[readLoc];
                readLoc++;
            }
            return outputArray;
        }
    }
}
