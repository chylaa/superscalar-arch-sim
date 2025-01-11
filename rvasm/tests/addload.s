.global _reset
.text

_reset:
	/* Init stack pointer */ 
    lui sp, %hi(__glob_stack_ptr$)
    addi sp, sp, %lo(__glob_stack_ptr$)
.extern _boot_datacopy
    call _boot_datacopy
    
.global _start
_start: 
	addi sp, sp,-16 	# allocate space for 4 registers
	sw ra, 12(sp)		# save return address at 0x107FC
	sw s0, 8(sp)		# save s0 at 0x107F8
	addi s0, sp, 16		# set s0 to 0x10800
	li a5, 42			# a5 = 42
	sw a5,-12(s0)		# save a5 (0x2A) at 0x107F4 | 17 2,286 0,438 7
	li a5, 69			# a5 = 69
	sw a5,-16(s0)		# save a5 (0x45) at 0x107F0
	la a5, result		# a5 = addr(0x1CAFE) [0x10000]
	lw a3,-12(s0)		# a3 = 42
	lw a4,-16(s0)		# a4 = 69
	add	a4, a3, a4		# a4 = 42 + 69 = 111
	sw a4, 0(a5)		# save a4 (0x6F) at addr(0x1CAFE) [0x10000]
	li a5, 1			# a5 = 1
	mv a0, a5			# a0 = 1
	lw ra, 12(sp)		# restore return address
	lw s0, 8(sp)		# restore s0
	addi sp, sp, 16		# deallocate space for 4 registers
	ret					# return to caller
 
.data
	result:
		.word 0x0001cafe
 	