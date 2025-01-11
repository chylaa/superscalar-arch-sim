.global _reset
.text

_reset: 
    la x15, __sdata$
    la x13, __glob_stack_ptr$              
    addi x13, x13, -4 # Address max
    li x1, 0x8000000 # End value 
    li x11, 0x01 # Shift amount
outer_loop:
    li x10, 0x02 # Start value
    beq x15, x13, end_outer_loop
inner_loop:
    sll x10, x10, x11
    bne x10, x1, inner_loop
    sw x10, 0(x15)
    addi x15, x15, 4 # next store address
    j outer_loop
end_outer_loop:
    ebreak