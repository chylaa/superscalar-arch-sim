#include "../../simlib/stddef.h"

#define  nil		0
#define	 false		0
#define  true		1
#define  intmmbase 	1.46f
#define  mmbase 	0.0f

#define repeat 1

    /* Intmm, Mm */
#define rowsize 	 10
float value, fixed, floated;

    /* global */
long    seed;  /* converted to long for 16 bit WR*/

    /* Intmm, Mm */

int   ima[rowsize+1][rowsize+1], imb[rowsize+1][rowsize+1], imr[rowsize+1][rowsize+1];
float rma[rowsize+1][rowsize+1], rmb[rowsize+1][rowsize+1], rmr[rowsize+1][rowsize+1];

void Initrand () {
    seed = 74755L;   /* constant to long WR*/
}

int Rand () {
    seed = (seed * 1309L + 13849L) & 65535L;  /* constants to long WR*/
    return( (int)seed );     /* typecast back to int WR*/
}
    /* Multiplies two real matrices. */

void rInitmatrix ( float m[rowsize+1][rowsize+1] ) {
	int temp, i, j;
	for ( i = 1; i <= rowsize; i++ )
	    for ( j = 1; j <= rowsize; j++ ) {
	    	temp = Rand();
			m[i][j] = (float)(temp - (temp/120)*120 - 60)/3;
        }
}

void rInnerproduct(float *result, float a[rowsize+1][rowsize+1], float b[rowsize+1][rowsize+1], int row, int column) {
	/* computes the inner product of A[row,*] and B[*,column] */
	int i;
	*result = 0.0f;
	for (i = 1; i<=rowsize; i++) *result = *result+a[row][i]*b[i][column];
}

void Mm (int run)    {
    int i, j;
    Initrand();
    rInitmatrix (rma);
    rInitmatrix (rmb);
    for ( i = 1; i <= rowsize; i++ )
		for ( j = 1; j <= rowsize; j++ ) 
			rInnerproduct(&rmr[i][j],rma,rmb,i,j);
}

int main()
{
	int i;
	for (i = 0; i < repeat; i++) Mm(i);
	EXIT(0);
    return 0;
}
