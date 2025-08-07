#ifndef STDDEF_H
#define STDDEF_H

/* Define as 1 if .data segment is provided to ROM after .text section and should be copied to RAM on crt0 procedrue.*/
#define DATACOPY 1

/* Linker script symbol - address of reset vector (entry point) */
extern unsigned int _reset;
/* Linker script symbol - address of stack end */
extern unsigned int _estack;           
/* Linker script symbol - address of stack start */
extern unsigned int _sstack;   
/* Linker script symbol - end address of .text section */
extern unsigned int _program_end;
/* Linker script symbol - start address of .data section */
extern unsigned int _sdata;
/* Linker script symbol - end address of .data section*/
extern unsigned int _edata;
/* Linker script symbol - size of declared RAM memory*/
extern unsigned int __rom_length;

/* NULL pointer */
#define NULL ((void*)0)
/* Offset of member in struct */
#define offsetof(_type, member) ((unsigned int) &((_type *)0)->member)

/* Write unsigned int 'value' to memory at 'addr' macro */
#define WRITE(addr, value) (*((volatile unsigned int*)(addr)) = (value))
/* Read value unsigned int from memory at 'addr' macro */
#define READ(addr) (*(volatile unsigned int*)addr) 
/* Copies unsigned int value from address 'src' into 'dst' address */
#define MEMCPY(src, dst) (WRITE(dst, READ(src)))

/* Write unsigned char 'value' to memory at 'addr' macro */
#define WRITE_BYTE(addr, value) (*((volatile unsigned char*)(addr)) = (value))
/* Read uint8_t value from memory at 'addr' macro */
#define READ_BYTE(addr) (*(volatile unsigned char*)addr) 
/* Copies uint8_t value from address 'src' into 'dst' address */
#define BYTECPY(src, dst) (WRITE_BYTE(dst, READ_BYTE(src)))

/* Write 'value' of typeof(value) to memory at 'addr'*/
#define WRITE_T(addr, value) (*((volatile typeof(value)*)(addr)) = (value))

/* WORD bytesize */
#define WORDSIZE sizeof(unsigned int)  
/* RAM start address */     
#define ORIGIN (&_sdata);                    
/* Stack size base on linker script symbols  */
#define STACK_SIZE (_sstack - _estack)        
/* ADDRESS of first free R/W RAM address + sizeof(WORD) after stack */
#define FIRST_FREE_ADDR ((&_sstack)+1)  
/* ADDRESS where exit code should be written when using EXIT(code) macro*/
#define EXIT_CODE_DESTINATION (FIRST_FREE_ADDR)

/* Returns 1/0 at 'bit' position of 'value' */
#define IS_BIT_SET(value, bit) (((value) & (1 << bit)) >> bit)
/* Sets specified 'bit' of 'value' to 1 */
#define SET_BIT(value, bit) (value = ((value) | (1 << bit)))
/* Resets specified 'bit' of 'value' to 0 */
#define RESET_BIT(value, bit) (value = ((value) & ~(1 << bit)))

/* Writes provided exit code into EXIT_CODE_DESTINATION address*/
#define EXIT(code) (WRITE(EXIT_CODE_DESTINATION, code))

__attribute__((always_inline))
/* Inserts asm inline that jumps to _reset entry label */
inline void __reset() { asm volatile("j _reset" ::); }
__attribute__((always_inline)) 
/* Inserts asm inline that halts the simulated processor */
inline void __ebreak() { asm volatile("ebreak" ::); }
/* Inserts asm inline that writes "code" into EXIT_CODE_DESTINATION address and halts simulation*/
__attribute__((always_inline))
inline void __exit(int code) {
    EXIT(code);
    __ebreak();
}   

#endif // STDDEF_H