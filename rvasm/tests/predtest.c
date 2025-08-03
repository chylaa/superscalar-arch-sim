#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

#define ARR_SIZE (4096)

uint64_t sum = 0;
int64_t seed = 74755L;
int32_t arr[ARR_SIZE]; 

__attribute__((always_inline)) 
inline void initrand () {
    seed = 74755L;
}
__attribute__((always_inline)) 
inline int32_t rand () {
    return (int32_t)(seed = (seed * 1309L + 13849L) & 65535L);  
}

int main() {

    int i;
    initrand();
    for (i = 0; i < ARR_SIZE; ++i){
        arr[i] = rand();
    }
    for (i = 0; i < ARR_SIZE; ++i){
        sum += arr[i];
    }
    return 0;
}