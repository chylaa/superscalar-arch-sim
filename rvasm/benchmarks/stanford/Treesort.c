#include "../../simlib/stddef.h"

#define  nil		0
#define	 false		0
#define  true		1
#define  towersbase 2.39f

    /* Bubble, Quick */
#define sortelements 500
#define srtelements  500
#define repeat 1

   /* tree */
struct node {
	struct node *left,*right;
	int val;
};

float value, fixed, floated;

    /* global */
long    seed;  /* converted to long for 16 bit WR*/
    /* tree */
struct node *tree;

    /* Bubble, Quick */
int sortlist[sortelements+1], biggest, littlest, top;

static int* mallocptr = FIRST_FREE_ADDR; 
void* malloc(int size) {
    void* ptr = mallocptr;
    mallocptr += size;
    return ptr;
}

void Initrand () {
    seed = 74755L;   /* constant to long WR*/
}

int Rand () {
    seed = (seed * 1309L + 13849L) & 65535L;  /* constants to long WR*/
    return( (int)seed );     /* typecast back to int WR*/
}

/* Sorts an array using treesort */
void tInitarr() {
	int i;
	long temp;  /* converted temp to long for 16 bit WR*/
	Initrand();
	biggest = 0; littlest = 0;
	for ( i = 1; i <= sortelements; i++ ) {
	    temp = Rand(); 
	    /* converted constants to long in next stmt, typecast back to int WR*/
	    sortlist[i] = (int)(temp - (temp/100000L)*100000L - 50000L);
	    if ( sortlist[i] > biggest ) biggest = sortlist[i];
	    else if ( sortlist[i] < littlest ) littlest = sortlist[i];
	}
}

void CreateNode (struct node **t, int n) {
		*t = (struct node *)malloc(sizeof(struct node)); 
		(*t)->left = nil; (*t)->right = nil;
		(*t)->val = n;
}

void Insert(int n, struct node *t) {
	/* insert n into tree */
	if ( n > t->val ) 
		if ( t->left == nil ) CreateNode(&t->left,n);
		else Insert(n,t->left);
	else if ( n < t->val )
		if ( t->right == nil ) CreateNode(&t->right,n);
		else Insert(n,t->right);
}

int Checktree(struct node *p) {
    /* check by inorder traversal */
    int result;
    result = true;
	if ( p->left != nil ) 
	   if ( p->left->val <= p->val ) result=false;
	   else result = Checktree(p->left) && result;
	if ( p->right != nil )
	   if ( p->right->val >= p->val ) result = false;
	   else result = Checktree(p->right) && result;
	return( result);
} /* checktree */

void Trees(int run) {
    int i;
    tInitarr();
    tree = (struct node *)malloc(sizeof(struct node)); 
    tree->left = nil; tree->right=nil; tree->val=sortlist[1];
    for ( i = 2; i <= sortelements; i++ )
		Insert(sortlist[i],tree);
    if ( ! Checktree(tree) ) 
        __exit(1);
}

int main()
{
	int i;
	for (i = 0; i < repeat; i++) Trees(i);
	EXIT(0);
    return 0;
}
