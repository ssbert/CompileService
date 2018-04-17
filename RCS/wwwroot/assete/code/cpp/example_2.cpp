#include <iostream>
using namespace std;

//得分点1：自定义函数 BubbleSort
void BubbleSort(int arr[], int length)
{
    //得分点2：嵌套 for 循环
     int temp;
     for (int i = 0; i < length; ++i)
     {
          for (int j = 0; j < length - i - 1; ++j)
          {
               if (arr[j] > arr[j + 1])
               {
                    temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
               }
          }
     }
}
 
int main ()
{
    int arr[10] = {2, 4, 1, 0, 8, 4, 8, 9, 20, 7};

    BubbleSort(arr, sizeof(arr) / sizeof(arr[0]));

    int i = 0; 
    while (i < sizeof(arr) / sizeof(arr[0]))
    {
        cout << arr[i] << " ";
        i++;
    }

    cout << endl;

    return 0;
}