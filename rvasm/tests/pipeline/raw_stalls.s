.global _reset
.text

_reset: 
    li x5, 0xdeacf3f1 
    lui x15, %hi(__sdata$)
    addi x15, x15, %lo(__sdata$)
    add x2, x0, x15     # x2 = __sdata$ (0x10000)
    lw x1, 0(x2)        # x1 = __sdata$[0] (0xdeadbeef)
    sub x4, x1, x5      # x4 = 0xcafe
    and x6, x1, x7      # x6 = 0x5eadbeef
    or x8, x1, x9       # x8 = 0xffffffff
    ebreak
.data
t: .word 0xdeadbeef