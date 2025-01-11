.global _reset
.text 

_reset:
    li x3, 20           # loop iterations = 5 (20/4)
    la x1, __sdata$     # "array" pointer
    add x3, x1, x3      # end pointer
loop:
    lw x2, 0(x1)        # load "array" element
    addi x2, x2, 1      # increment data 
    sw x2, 0(x1)        # store result
    addi x1, x1, 4      # increment pointer
    bne x1, x3, loop    # branch until end
    sw x3, 0(x1)        # store end pointer (should be 0x....14)
    nop
    nop
    nop
    ebreak