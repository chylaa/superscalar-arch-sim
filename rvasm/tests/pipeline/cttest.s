.global _reset
.text

_reset:
    add x6, x7, x8
    add x7, x8, x9
    beq x6, x7, _reset
    sub x6, x7, x28
    j label
    addi x0, x6, -2023
    sub x7, x28, x29
label:
    beq x6, x7, _reset
    ebreak
