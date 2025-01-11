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
	/* Init stack pointer */ 
    lui sp, %hi(__glob_stack_ptr$)
    addi sp, sp, %lo(__glob_stack_ptr$)
    /* Copy initialized vars*/
.extern _boot_initdata
    call _boot_initdata
    /* Jump to main */
.extern main
    call main
/* Force simulator-specific ebreak exception */
    ebreak
/*Sanity no-ops so EBREAK will def. have chance to be detected*/
    nop
    nop
    nop
    nop
    nop