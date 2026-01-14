/* Set of initialization startup routines required before calling the program's main function */

.global _reset
.global _start

.section .text
.section .reset_vector
_reset:
    j _start /* Jump over the reset vector and boot code */

.section .boot_initdata
    /* Placeholder for boot.c or other startup code to copy initialized data */

_start:
    /* Save potential calling code stack pointer and return address */
    mv s0, sp
    mv s1, ra
	/* Init stack pointer */ 
    lui sp, %hi(__glob_stack_ptr$)
    addi sp, sp, %lo(__glob_stack_ptr$)
    /* Copy initialized vars*/
.extern _boot
    call _boot
    /* Jump to main */
.extern main
    call main
.extern _fini
    call _fini
    /*  Restore potential calling code stack pointer and return address */
    mv sp, s0
    mv ra, s1
/* Jump to simlib exit routine */
.extern _exit
    jal x0, _exit
/*Sanity no-ops */
    nop
    nop
    nop
    nop
    nop