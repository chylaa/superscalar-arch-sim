#ifndef SIMLIB_HPP
#define SIMLIB_HPP

namespace std {

    typedef int ptrdiff_t;
    typedef unsigned size_t;

    template<class T>
    constexpr 
    const T& min(const T& a, const T& b)
    {
        return (b < a) ? b : a;
    }

    template<class T> 
    const T& max(const T& a, const T& b)
    {
        return (a < b) ? b : a;
    }

    constexpr
    unsigned int strlen(const char* str) { 
        unsigned int len = 0;
        while (*str++) len += 1;
        return len;
    }

    constexpr char* strcpy(char* dest, const char* src)
    {
        char* d = dest;
        while ((*d++ = *src++));
        return dest;
    }

    constexpr char* strncpy(char* dest, const char* src, unsigned n)
    {
        unsigned i = 0;
        for (; i < n && src[i]; ++i)
            dest[i] = src[i];
        if (i < n)
            dest[i] = '\0';
        return dest;
    }

}

#endif /* SIMLIB_HPP */