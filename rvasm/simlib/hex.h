#ifndef HEX_H
#define HEX_H

#include "stdint.h"

static const char HEXCHARS[17] = "0123456789ABCDEF";

// Returns '1' if 'c' is a hexadecimal digit ([0-9], [a-f], [A-F])
int isxdigit(int c)
{
  return (
          ((c >= '0') && (c <= '9')) || 
          ((c >= 'a') && (c <= 'f')) ||
          ((c >= 'A') && (c <= 'F'))
         );
}

/* Converts 32bit unsigned integer to 8-character NULL-terminated string placed in 'buffer'. Returns 'buffer'. */
char* u32tohex(uint32_t num, char* buffer) {
    int i = 8;
    buffer[i] = '\0';
    for ( ; i > 0; num >>= 4)
       buffer[--i] = HEXCHARS[num&0xF];
    return buffer;
}


/* Converts hex string from 'input' to 32bit unsigned integer. 
Takes up to 8 lower/upper case characters, with/wihout '0x' prefix for input. 
NOTE: 'len' must take include the 2 bytes for '0x' prefix if present. Hex can contain leading zeroes.
Returns 0 if conversion failed. 
*/
int hextou32(const char *input, int len, uint32_t *out)
{
    if (len <= 0) {
        return 0;
    }
    if (len > 2 && input[0] == '0' && (input[1] == 'X' || input[1] == 'x')) {
        input += 2;
        len -= 2;
    }
    const int MAX_CHARS = 2 * sizeof(uint32_t);
    if (len > MAX_CHARS) {
        len = MAX_CHARS;
    } 

    uint32_t num = 0;
    for (int i = 0; len > 0; --len, ++i) {
        uint8_t halfb;
        char c = input[len-1];
        if (c >= '0' && c <= '9') {
            halfb = c - '0';
        } else if (c >= 'A' && c <= 'F') {
            halfb = c - 'A' + 0x0A;
        } else if (c >= 'a' && c <= 'f') {
            halfb = c - 'a' + 0x0A;
        } else { // invalid character
            return 0;
        }
        num += (halfb << (4*i));
    }
    *out = num;
    return 1;
}

#endif /* __HEX_H__ */