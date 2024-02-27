// Example program
#include <iostream>
#include <string>

using namespace std;

const int MAX = 1000;

int a[MAX][MAX];
int b[MAX];

void LietKe(int cot) {
    int hang = cot * cot;
    for(int i = 0; i < hang; i++) {
        for (int j = 0; j < cot; j++) {
            a[i][j] = (i >> j) & 1;
        }
    }
}

void XuatKQ(int cot) {
    int hang = cot * cot;
    
    for (int i = 0; i < cot; i++) {
        b[i] = i + 1;
    }
    
    for (int i = 0; i < hang; i++) {
        for (int j = 0; j < cot; j++) {

                if (a[i][j] == 1) {
                    cout << "| " << j + 1;
                }

            
        }
        cout << endl;
    }
}

int main()
{
    int count = 4;
    
    LietKe(count);
    
    XuatKQ(count);
    
    return 0;
}
