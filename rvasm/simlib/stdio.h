#ifndef STDIO_H
#define STDIO_H

#include "io.h"

int putc(char c) {
    while (io_write(c) == IO_ERROR);
    return c; 
}

int getc() {
    int c;
    while ((c = io_read()) == IO_ERROR); 
    return c;
}

int puts(const char* s) {
    int i; char c;
    for (i = 0; c = s[i]; ++i)
        putc(c);
    return i;
}

#endif /* STDIO_H */