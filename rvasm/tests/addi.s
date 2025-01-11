.global _reset
.text

_reset:                    /* x0  = 0    0x000 */
    /* Test ADDI */
    addi x1 , x0,   1000  /* x1  = 1000 0x3E8 */
    addi x2 , x1,   2000  /* x2  = 3000 0xBB8 */
    addi x3 , x2,  -1000  /* x3  = 2000 0x7D0 */
    addi x4 , x3,  -2000  /* x4  = 0    0x000 */
    addi x5 , x4,   1000  /* x5  = 1000 0x3E8 */

    la x6, variable       /* x6  = 0x10000000 */
    addi x6, x6, 4        /* x6  = 0x10000004 */
    ebreak
    nop
    nop
    nop
    nop
    nop

.data
variable:
	.word 0xdeadbeef
                    