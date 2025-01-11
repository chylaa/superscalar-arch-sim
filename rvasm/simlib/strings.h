#ifndef STRINGS_H
#define STRINGS_H

typedef const char* string;

/*Null terminator character*/
#define EOS '\0'
/*End-Of-Line*/
#define EOL '\n'

/*Returns lenght of NULL-terminated char sequence.*/
unsigned int strlen(string str) { 
    unsigned int len = 0;
    while (*str++) len += 1;
    return len;
}

/*Compares two NULL-terminated char sequences.*/
int strcmp(string str1, string str2) {
    while (*str1 && *str2 && *str1 == *str2) {
        str1++;
        str2++;
    }
    return *str1 - *str2;
}

/* Converts unsigned integer to NULL-terminated string of maximum length 11 */
string itoa(unsigned int num) {
    static unsigned char buffer[11];
    unsigned char i = 10;
    buffer[i] = EOS;
    do {
        buffer[--i] = num % 10 + '0';
        num /= 10;
    } while (num);
    return &buffer[i];
}

void* memcpy (void* dest, const void* src, unsigned int n) {
    unsigned char* d = (unsigned char*)dest;
    unsigned char* s = (unsigned char*)src;
    while (n--) {
        *d++ = *s++;
    }
    return dest;
}

void* memset(void* dest, int val, unsigned int n) {
    unsigned char* d = (unsigned char*)dest;
    while (n--) {
        *d++ = val;
    }
    return dest;
}

/* Copies passed string to another location in memory, starting at given address, until \0 character. */
void strcpy(char* src, char* dst) {
    while (*src) {
        *dst = *src;
        src++;
        dst++;
    }
    *dst = EOS;
}

#endif