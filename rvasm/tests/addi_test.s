.global _reset
.text

_reset:                   
    lui	    x15,0x11       # x15 = 0x11000 (69632) 
    lw	    x15,-2040(x15) # x15 = 0x10808 (67592)
    addi	x15,x15,4      # x15 = 0x1080c (67596)
    addi	x9,x15,0       # x9  = x15

    lw x6, DeadBeef
    addi x6, x6, 4
    lh x7, Feed
    addi x7, x7, -1

.data
	DeadBeef: .word 0xdeadbeef
	BaddCafe: .word 0xbaddcafe
	Feed: 	  .half 0xfeed
    Xdd :     .byte 0xdd