#include <iostream>
using namespace std;
 
int main ()
{
    //得分点1：使用 for 循环
    int i = 0;
    for(i = 0; i < 3; i++)
    {
        printf("for 循环 index: %d.\n", i);
    }

    //得分点2：使用 while 循环
    int j = 0;
    while(j < 4)
    {
        printf("while 循环 count: %d.\n", j);
        j++;
    }

    //得分点3：使用 if 判断
    if (i < j) 
    {
        printf("for 循环次数小于 while 循环次数.\n");
    }
    
    return 0;
}