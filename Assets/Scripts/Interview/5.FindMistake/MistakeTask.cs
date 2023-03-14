using UnityEngine;

namespace Interview._5.FindMistake
{
    public class MistakeTask
    {
        private void Sum()
        {
            int[] numbers = { 1, 2, 3, 4, 5 };
            int sum = 0;
            
            for (int i = 0; i <= numbers.Length; i++) 
                sum += numbers[i];

            Debug.Log($"The sum of the numbers is: {sum}");
        }
    }
}