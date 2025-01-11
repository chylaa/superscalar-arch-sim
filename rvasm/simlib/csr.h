#ifndef CSR_H
#define CSR_H

/* Machine Control Status Registers */
#define CSR_MISA     0x301
#define CSR_MVENDORID 0xF11
#define CSR_MARCHID  0xF12
#define CSR_MIMPID   0xF13
#define CSR_MHARTID  0xF14
#define CSR_MSTATUS  0x300
#define CSR_MISA     0x301
#define CSR_MEDELEG  0x302
#define CSR_MIDELEG  0x303
#define CSR_MIE      0x304
#define CSR_MTVEC    0x305
#define CSR_MCOUNTEREN 0x306
#define CSR_MSCRATCH 0x340
#define CSR_MEPC     0x341
#define CSR_MCAUSE   0x342
#define CSR_MTVAL    0x343
#define CSR_MIP      0x344
#define CSR_MBASE    0x380
#define CSR_MBOUND   0x381
#define CSR_MIBASE   0x382
#define CSR_MIBOUND  0x383
#define CSR_MDBASE   0x384
#define CSR_MDBOUND  0x385
#define CSR_MIBASE   0x382
#define CSR_MIBOUND  0x383
#define CSR_MDBASE   0x384
#define CSR_MDBOUND  0x385
#define CSR_HTVAL    0x3A0
#define CSR_HTINST   0x3A1
#define CSR_HTVIRT   0x3A2
#define CSR_HTCYCLE  0x3A3
#define CSR_HTLB     0x3A4
#define CSR_HTMISC   0x3A5
#define CSR_HPMSTAR  0x3A6
#define CSR_HPMEVENT 0x3A7
#define CSR_MVENDID  0xF11
#define CSR_MARCHID  0xF12
#define CSR_MIMPID   0xF13
#define CSR_MHARTID  0xF14

/* Machine Control Status Registers */

// Inline function to read value from CSR
inline unsigned int __attribute__((always_inline)) read_csr(int csr_num) ;

// Inline function to write a value to a CSR
inline void __attribute__((always_inline)) write_csr(int csr_num, unsigned int value);

// Inline function to set a bit in a CSR
inline void __attribute__((always_inline)) set_csr_bit(int csr_num, unsigned int bit);

// Inline function to clear a bit in a CSR
inline void __attribute__((always_inline)) clear_csr_bit(int csr_num, unsigned int bit);

#endif /* CSR_H */