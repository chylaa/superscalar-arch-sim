#include "../simlib/stddef.h"
#include "../simlib/stdint.h"

typedef struct Point {
    int x;
    int y;
} Point;

typedef struct Triangle {
    Point p1;
    Point p2;
    Point p3;
} Triangle;

uint32_t abs(int32_t x) {
    return (x < 0) ? -x : x;
}

uint32_t area(const Triangle* t, uint32_t* reminder) {
    uint32_t a = abs(t->p1.x * (t->p2.y - t->p3.y) + t->p2.x * (t->p3.y - t->p1.y) + t->p3.x * (t->p1.y - t->p2.y));
    *reminder = a % 2;
    return a / 2;
}


int32_t is_inside(const Triangle* t, const Point* p) {
    uint32_t* unused = (uint32_t*)FIRST_FREE_ADDR;
    uint32_t A = area(t, unused);
    uint32_t A1 = area(&(Triangle){*p, t->p2, t->p3}, unused);
    uint32_t A2 = area(&(Triangle){t->p1, *p, t->p3}, unused);
    uint32_t A3 = area(&(Triangle){t->p1, t->p2, *p}, unused);
    return (A == A1 + A2 + A3) ? 1 : 0;
}

int32_t is_right_triangle(const Triangle* t) {
    int32_t a = (t->p1.x - t->p2.x) * (t->p1.x - t->p2.x) + (t->p1.y - t->p2.y) * (t->p1.y - t->p2.y);
    int32_t b = (t->p1.x - t->p3.x) * (t->p1.x - t->p3.x) + (t->p1.y - t->p3.y) * (t->p1.y - t->p3.y);
    int32_t c = (t->p2.x - t->p3.x) * (t->p2.x - t->p3.x) + (t->p2.y - t->p3.y) * (t->p2.y - t->p3.y);
    return (a == b + c || b == a + c || c == a + b) ? 1 : 0;
}

int32_t main() {
    uint32_t* result = (uint32_t*)FIRST_FREE_ADDR;
    const Point inside = {2, 3};
    const Triangle t = {{1, 1}, {1, 6}, {4, 1}};
    
    int32_t rem = 0;
    uint32_t a = area(&t, &rem);
    int32_t inside_result = is_inside(&t, &inside);
    int32_t right_result = is_right_triangle(&t);

    WRITE(result++, a);
    WRITE(result++, rem);
    WRITE(result++, inside_result);
    WRITE(result++, right_result);
    WRITE(result, 0xDEADBEEF);
}


/*
    P2(1,6)
    |\ 
    | \
    |  \
    |   \
    |    \
    |___  \
    | . |  \
    +---+---+
  P1(1,1)  P3(4,1)  

    o area = 7 (7,5)
    o reminder = 1
    o inside = 1
    o right = 1
*/