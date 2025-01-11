#include "..\..\simlib\stddef.h"
#include "..\..\simlib\strings.h"

#include "matrix.h"

#define NUM_ITERATIONS 10
#define N_COLUMNS  5
#define M_ROWS     10

void matrix_mul();

#define MULTIPLY_MATRIX(bits) \
int multiply_##bits(Matrix_##bits * a, Matrix_##bits * b, Matrix_##bits * res)     \
{                                                                                  \
  if(a->n != b->m)                                                                 \
    return -1;                                                                     \
  if(res->m < a->m || res->n < b->n)                                               \
    return -2;                                                                     \
  res->m = a->m;                                                                   \
  res->n = b->n;                                                                   \
                                                                                   \
  memset((void*)(res->data), 0, res->n * res->m);                                  \
                                                                                   \
  int i,j,k;                                                                       \
  for(i = 0; i < res->m; i++)                                                      \
  {                                                                                \
    for(j = 0; j < res->n; j++)                                                    \
    {                                                                              \
      for(k = 0; k < a->n ;k++)                                                    \
        res->data[i * res->n + j] += a->data[i * a->n + k] * b->data[k * b->n + j];\
    }                                                                              \
  }                                                                                \
  return 0;                                                                        \
}

#define FAST_MULTIPLY_MATRIX(bits) \
int fast_multiply_##bits(Matrix_##bits * a, Matrix_##bits * b, Matrix_##bits * res) \
{                                                                                  \
  if(a->n != b->m)                                                                 \
    return -1;                                                                     \
  if(res->m < a->m || res->n < b->n)                                               \
    return -2;                                                                     \
  res->m = a->m;                                                                   \
  res->n = b->n;                                                                   \
                                                                                   \
  memset((void*)(res->data), 0, res->n * res->m);                                  \
  int##bits##_t *res_data = res->data, *a_data, *a_data_save, *b_data;             \
  int i,j,k;                                                                       \
  for(i = 0, a_data_save = a->data ; i < res->m; i++, a_data_save += a->n)         \
  {                                                                                \
    for(j = 0; j < res->n; j++, res_data++)                                        \
    {                                                                              \
      b_data = b->data + j;                                                        \
      for(k = 0, a_data = a_data_save; k < a->n ;k++, b_data += b->n, a_data++)    \
        *res_data += (*a_data) * (*b_data);                                        \
    }                                                                              \
  }                                                                                \
  return 0;                                                                        \
}

MULTIPLY_MATRIX(8)
MULTIPLY_MATRIX(16)
MULTIPLY_MATRIX(32)
FAST_MULTIPLY_MATRIX(8)
FAST_MULTIPLY_MATRIX(16)
FAST_MULTIPLY_MATRIX(32)

#define SET_MATRIX(bits)                                   \
void set_matrix_##bits(Matrix_##bits *m, int max)          \
{                                                          \
  int i,j;                                                 \
  for(i = 0; i < m->m; i++)                                \
   for(j = 0; j < m->n; j++)                               \
    m->data[i * m->n + j] = (i * m->n + j + 1) % max;      \
}

SET_MATRIX(8)
SET_MATRIX(16)
SET_MATRIX(32)

void put_result(int id, int value) {
    uint32_t* base = FIRST_FREE_ADDR;
    WRITE_T(base+id, value);
}

void matrix_mul()
{
    int32_t matrix_a[M_ROWS * N_COLUMNS];
    int32_t res_matrix[M_ROWS * M_ROWS];
    int ret,i;

    Matrix_8 a_8 = {(int8_t*)matrix_a, M_ROWS, N_COLUMNS};
    Matrix_8 b_8 = {(int8_t*)matrix_a, N_COLUMNS, M_ROWS};
    Matrix_8 res_8 = {(int8_t*)res_matrix, M_ROWS, M_ROWS};

    Matrix_16 a_16 = {(int16_t*)matrix_a, M_ROWS, N_COLUMNS};
    Matrix_16 b_16 = {(int16_t*)matrix_a, N_COLUMNS, M_ROWS};
    Matrix_16 res_16 = {(int16_t*)res_matrix, M_ROWS, M_ROWS};

    Matrix_32 a_32 = {matrix_a, M_ROWS, N_COLUMNS};
    Matrix_32 b_32 = {matrix_a, N_COLUMNS, M_ROWS};
    Matrix_32 res_32 = {res_matrix,M_ROWS, M_ROWS};


    set_matrix_8(&a_8, 8);
    for(i = 0; i < NUM_ITERATIONS; i++)
    {
        ret = multiply_8(&a_8,&b_8,&res_8);
    }
    put_result(0, ret);

    res_8.m = M_ROWS; res_8.n = M_ROWS;
    for(i = 0; i < NUM_ITERATIONS; i++)
    {
        ret = fast_multiply_8(&a_8,&b_8,&res_8);
    }
    put_result(1, ret);

    set_matrix_16(&a_16, 30);
    for(i = 0; i < NUM_ITERATIONS; i++)
    {
        ret = multiply_16(&a_16,&b_16,&res_16);
    }
    put_result(2, ret);

    res_16.m = M_ROWS; res_16.n = M_ROWS;
    for(i = 0; i < NUM_ITERATIONS; i++)
    {
        ret = fast_multiply_16(&a_16,&b_16,&res_16);
    }
    put_result(3, ret);

    set_matrix_32(&a_32, 100);
    for(i = 0; i < NUM_ITERATIONS; i++)
    {
        ret = multiply_32(&a_32,&b_32,&res_32);
    }
    put_result(4, ret);

    res_32.m = M_ROWS; res_32.n = M_ROWS;
    for(i = 0; i < NUM_ITERATIONS; i++)
    {
        ret = fast_multiply_32(&a_32,&b_32,&res_32);
    }
    put_result(5, ret);
}

int main()
{
    matrix_mul();
}