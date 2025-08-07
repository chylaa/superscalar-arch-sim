#ifndef STRINGS_H
#define STRINGS_H

/*Null terminator character*/
#define EOS '\0'
/*End-Of-Line*/
#define EOL '\n'

/*Returns lenght of NULL-terminated char sequence.*/
unsigned int strlen(const char* str) { 
    unsigned int len = 0;
    while (*str++) len += 1;
    return len;
}

/*Compares two NULL-terminated char sequences.*/
int strcmp(const char* str1, const char* str2) {
    while (*str1 && *str2 && *str1 == *str2) {
        str1++;
        str2++;
    }
    return *str1 - *str2;
}

/* Reverses in-place string 'str' of specified length 'len'. Returns 'str'. */
char* strreverse(char* str, unsigned len)
{
    if (len <= 1) 
        return str;

    char* src = str;
    char* dst = src + len - 1;
    while (src < dst) {
        char tmp = *src;
        *src++ = *dst;
        *dst-- = tmp;
    }
    return str;
}

/* Converts unsigned integer 'num' to NULL-terminated string placed into 'buffer'. Returns length of created string. */
unsigned itoa(unsigned int num, char* buffer) {
    unsigned i;
    do {
        buffer[i++] = num % 10 + '0';
        num /= 10;
    } while (num);
    strreverse(buffer, i)[i] = EOS;
    return i;
}

void* memcpy(void* dest, const void* src, unsigned int n) {
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