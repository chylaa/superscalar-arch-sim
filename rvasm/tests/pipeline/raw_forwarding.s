.global _reset
.text

_reset: 
    addi x2, x0, 0x00000002
    addi x3, x0, 0x00000003
    addi x5, x0, 0x00000005
    addi x7, x0, 0x00000007
    addi x9, x0, 0x00000009
    addi x11, x0, 0x0000000B
    add x1, x2, x3      # x1 = 5
    sub x4, x1, x5      # x4 = 0
    and x6, x1, x7      # x6 = 5
    or x8, x1, x9       # x8 = 13
    xor x10, x1, x11    # x10 = 14
    lui x15, %hi(__sdata$)
    addi x15, x15, %lo(__sdata$)
    add x1, x0, x15     # x1 = __sdata$ (0x10000)
    lw x4, 0(x1)        # x4 = 0xdeadbeef       lw [rd], 0[rs1]
    sw x4, 12(x1)       # t+12 = 0xdeadbeef     sw [rs2], 12[rs1]
    ebreak

.data
t: .word 0xdeadbeef