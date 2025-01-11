#include "../simlib/stdint.h"
#include "../simlib/stddef.h"

# if __has_builtin(__builtin_nanf)
#   define NaN (__builtin_nanf(""))
# else
#   define NaN (0.0f / 0.0f)
# endif
# if __has_builtin(__builtin_inff)
#   define INFINITY (__builtin_inff())
# else
#   define INFINITY 1e10000f
# endif

int __isnan (float x) {    
  return (0x7F800000 == ((*(uint32_t*)&x) & 0x7F800000)) && ((*(uint32_t*)&x) & 0x007FFFFF);
}

void positive_addition(uint32_t* addr){
    float pos_add = 3.5f + 2.5f;  // Expected: 6.0
    WRITE_T(addr, pos_add);
}

void negative_addition(uint32_t* addr){
    float neg_add = -3.5f + -2.5f;  // Expected: -6.0
    WRITE_T(addr, neg_add);
}

void positive_subtraction(uint32_t* addr){
    float pos_sub = 3.5f - 2.5f;  // Expected: 1.0
    WRITE_T(addr, pos_sub);
}

void negative_subtraction(uint32_t* addr){
    float neg_sub = -3.5f - -2.5f;  // Expected: -1.0
    WRITE_T(addr, neg_sub);
}

void multiplication(uint32_t* addr){
    float mul = 3.5f * 2.5f;  // Expected: 8.75
    WRITE_T(addr, mul);
}

void division(uint32_t* addr){
    float div = 3.5f / 2.5f;  // Expected: 1.4
    WRITE_T(addr, div);
}

void greater_than(uint32_t* addr){
    uint32_t cmp_gt = (3.5f > 2.5f) ? 1 : 0;  // Expected: 1
    WRITE_T(addr, cmp_gt);
}

void less_than(uint32_t* addr){
    uint32_t cmp_lt = (3.5f < 2.5f) ? 1 : 0;  // Expected: 0
    WRITE_T(addr, cmp_lt);
}

void equal_to(uint32_t* addr){
    uint32_t cmp_eq = (3.5f == 3.5f) ? 1 : 0;  // Expected: 1
    WRITE_T(addr, cmp_eq);
}

void test_nan_eq_nan(uint32_t* addr){
    volatile float nanVal1 = NaN;
    volatile float nanVal2 = NaN;
    uint32_t cmp_nans = ((nanVal1 == nanVal2) ? 1 : 0);  // Expected 0
    WRITE_T(addr, cmp_nans);
}

void def_nan_is_nan(uint32_t* addr){
    WRITE_T(addr, __isnan(NaN));   // Expected: 1
}

void inf_is_nan(uint32_t* addr){
    WRITE_T(addr, __isnan(INFINITY));  // Expected: 0
}

void produced_nan_is_nan(uint32_t* addr){
    volatile float test_nan_3_value = (0.0f / 0.0f); // Producing a NaN
    WRITE_T(addr, __isnan(test_nan_3_value)); // Expected: 1
}

void test_underflow(uint32_t* addr){
    volatile float test_underflow_1 = 1e-40f; // Smallest positive
    volatile float test_underflow_2 = 1e-6f;
    WRITE_T(addr, test_underflow_1 - test_underflow_2); // Expected: -1e-6f
}

void positive_zero(uint32_t* addr){
    float pos_zero = 0.0f;  // Expected: 0.0
    WRITE_T(addr, pos_zero);
}

void negative_zero(uint32_t* addr){
    float neg_zero = -0.0f;  // Expected: -0.0
    WRITE_T(addr, neg_zero);
}

void positive_infinity(uint32_t* addr){
    float pos_inf = INFINITY;  // Expected: Infinity
    WRITE_T(addr, pos_inf);
}

void negative_infinity(uint32_t* addr){
    float neg_inf = -INFINITY;  // Expected: -Infinity
    WRITE_T(addr, neg_inf);
}

int main() {
    uint32_t* addr = (FIRST_FREE_ADDR-1);
    positive_addition(++addr);      //  6.0
    negative_addition(++addr);      // -6.0
    positive_subtraction(++addr);   //  1.0
    negative_subtraction(++addr);   // -1.0
    multiplication(++addr);         //  8.75
    division(++addr);               //  1.4
    greater_than(++addr);           //  1
    less_than(++addr);              //  0
    equal_to(++addr);               //  1
    test_nan_eq_nan(++addr);        //  0     
    def_nan_is_nan(++addr);         //  1
    inf_is_nan(++addr);             //  0
    produced_nan_is_nan(++addr);    //  1
    test_underflow(++addr);         // -1e-6f
    positive_zero(++addr);          //  0.0
    negative_zero(++addr);          // -0.0
    positive_infinity(++addr);      //  Inf
    negative_infinity(++addr);      // -Inf
}