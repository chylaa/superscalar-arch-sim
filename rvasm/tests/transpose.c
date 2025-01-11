#include "../simlib/stddef.h"
#include <float.h>

# if __has_builtin(__builtin_inff)
#   define INFINITY (__builtin_inff())
# else
#   define INFINITY 1e10000f
# endif

inline float __attribute__((always_inline)) 
min(float a, float b) {
    return a < b ? a : b;
}
inline void __attribute__((always_inline)) 
memset(float* dest, float val, int n) {
    for (int i = 0; i < n; ++i) {
        dest[i] = val;
    }
}

void step(float* r, const float* d_, int n) {
    const int nb = 4;
    int na = (n + nb - 1) / nb;
    int nab = na*nb;

    // input data, padded
    float d[n*nab];
    // input data, transposed, padded
    float t[n*nab];

    memset(d, INFINITY, n*n*sizeof(float));
    memset(t, INFINITY, n*n*sizeof(float));

    #pragma omp parallel for
    for (int j = 0; j < n; ++j) {
        for (int i = 0; i < n; ++i) {
            d[nab*j + i] = d_[n*j + i];
            t[nab*j + i] = d_[n*i + j];
        }
    }

    #pragma omp parallel for
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) {
            // vv[0] = result for k = 0, 4, 8, ...
            // vv[1] = result for k = 1, 5, 9, ...
            // vv[2] = result for k = 2, 6, 10, ...
            // vv[3] = result for k = 3, 7, 11, ...
            float vv[nb];
            for (int kb = 0; kb < nb; ++kb) {
                vv[kb] = INFINITY;
            }
            for (int ka = 0; ka < na; ++ka) {
                for (int kb = 0; kb < nb; ++kb) {
                    float x = d[nab*i + ka * nb + kb];
                    float y = t[nab*j + ka * nb + kb];
                    float z = x + y;
                    vv[kb] = min(vv[kb], z);
                }
            }
            // v = result for k = 0, 1, 2, ...
            float v = INFINITY;
            for (int kb = 0; kb < nb; ++kb) {
                v = min(vv[kb], v);
            }
            r[n*i + j] = v;
        }
    }
}

int main() {
    const int n = 4;
    float d[n*n];
    float r[n*n];
    for (int i = 0; i < n*n; ++i) {
        d[i] = i % n;
    }
    step(r, d, n);
    return 0;
}