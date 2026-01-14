/* Base on https://github.com/darklife/darkriscv */
/* ~ MC 2024 */
/* -------------------------------------------------------------------------- */
__wordsize      = 4;            /* 4 bytes per word */
__stack_size    = 0x800;        /* required amount of stack */
__rom_origin    = 0x00000000;   /* start of ROM */
__rom_length    = 0x00010000;   /* length of ROM */
__ram_origin    = 0x00010000;   /* start of RAM */
__ram_length    = 0x08000000;   /* length of RAM */

/* Set target and entry point */
TARGET (elf32-littleriscv)
ENTRY (_reset)

MEMORY
{
    ROM (rwx) : ORIGIN = __rom_origin, LENGTH = __rom_length
    RAM (rwx) : ORIGIN = __ram_origin, LENGTH = __ram_length
}
SECTIONS
{
    __nop_pad_size = 64 - 1;
    .text : ALIGN(__wordsize)
    {   
        KEEP(*(.reset_vector)); /* not used now */
        KEEP(*(.boot_initdata));
        
        _preinit_array_start = .;
        KEEP(*(.preinit_array))
        _preinit_array_end = .;
        
        _init_array_start = .;
        KEEP(*(.init_array))
        _init_array_end = .;

        _fini_array_start = .;
        KEEP(*(.fini_array))
        _fini_array_end = .;
        
        *(.text .text.*);
        
        . = . + (__wordsize * __nop_pad_size); /* Add padding words between .text and .rodata, so cpu will not speculate over .rodata before EBREAK */
        LONG(0x00100073) /* Single failsafe EBREAK instruction (LE) so .data section will not be accessed by accident */
        
        _srodata = .;
        *(.srodata .srodata.*);
        *(.rodata .rodata.* ); 
        . = ALIGN(4); /* Allign used .edata to 4-byte boundary */
        _erodata = .;
        _program_end = .;

    } > ROM =0x13000000 /* Fill with NOP's (LE), so padding will be properly set */

    .data :
    {
        _sdata = .;
        *(.sdata .sdata.*);
        *(.data .data.*);
        *(.rela*);
        *(COMMON);
        _edata = .;
        . = ALIGN(4); /* Allign 32bit datas to 4-byte boundary */
        . = ALIGN(2); /* Allign 16bit datas to 2-byte boundary */
        . = ALIGN(1); /* Allign 8bit datas to 1-byte boundary  */
    } > RAM

    /* block starting symbol, holds place for statically allocated variables that are declared but have not been assigned a value */
    .bss (NOLOAD) : ALIGN(__wordsize)
    {
        _sbss = .;
        *(.sbss .sbss.* .bss .bss.*);
        _ebss = .;
    } > RAM

    .stack : ALIGN(__wordsize)
    {
        _estack = .;
        . = . + __stack_size;
        _sstack = .;
    } >RAM

    PROVIDE( __sdata$ = _sdata );
    PROVIDE( __edata$ = _edata );
    PROVIDE( __sbss$ = _sbss);
    PROVIDE( __ebss$ = _ebss);
    PROVIDE( __datasize$ = SIZEOF(.data) );
    PROVIDE( __glob_stack_ptr$ = _sstack );
}
  
                        
