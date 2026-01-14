#ifndef STRING_HPP
#define STRING_HPP

#include "simlib.hpp"
#include "string_view.hpp"

namespace std {

    /*
    The freestanding-friendly, heap-less std::string implementation.
    This is intentionally unsafe as fuck and not ABI-compatible with hosted std::string. 
    It is a teaching/demo string, suitable for simulator examples until I work out how to manage a heap on a simulator.
        
        > [!] unchecked writes (by design)
    */
    class string {
    public:
        using size_type = unsigned;

        constexpr string() noexcept
            : _M_str(nullptr), _M_len(0), _M_cap(0)
        {}

        constexpr string(char* buffer, size_type capacity) noexcept
        : _M_str(buffer), _M_len(0), _M_cap(capacity)
        {
            if (_M_str && _M_cap)
                _M_str[0] = '\0';
        }

        constexpr string(char* buffer, size_type capacity, const char* init) noexcept
            : _M_str(buffer), _M_len(0), _M_cap(capacity)
        {
            assign(init);
        }

        /* Copy constructor */
        string(const string&) = delete;

        /* Move constructor */
        constexpr string(string&& other) noexcept
            : _M_str(other._M_str),
            _M_len(other._M_len),
            _M_cap(other._M_cap)
        {
            other._M_str = nullptr;
            other._M_len = 0;
            other._M_cap = 0;
        }

        /* Copy assigment — deep copy */
        constexpr string& operator=(const string& other) noexcept
        {
            if (this == &other || !_M_str)
                return *this;

            const size_type n = std::min(other._M_len, _M_cap - 1);
            std::strncpy(_M_str, other._M_str, n);
            _M_len = n;
            _M_str[_M_len] = '\0';

            return *this;
        }

        /* Move assigment */
        constexpr string& operator=(string&& other) noexcept
        {
            if (this != &other) {
                _M_str = other._M_str;
                _M_len = other._M_len;
                _M_cap = other._M_cap;

                other._M_str = nullptr;
                other._M_len = 0;
                other._M_cap = 0;
            }
            return *this;
        }
        ~string() = default;

        /* Capacity */

        constexpr size_type size() const noexcept { return _M_len; }
        constexpr size_type length() const noexcept { return _M_len; }
        constexpr size_type capacity() const noexcept { return _M_cap; }
        constexpr bool empty() const noexcept { return _M_len == 0; }

        /* Element access (unchecked) */

        constexpr char& operator[](size_type pos) noexcept
        { return _M_str[pos]; }

        constexpr const char& operator[](size_type pos) const noexcept
        { return _M_str[pos]; }

        constexpr char& at(size_type pos) noexcept
        { return _M_str[pos]; }

        constexpr const char& at(size_type pos) const noexcept
        { return _M_str[pos]; }

        constexpr char& front() noexcept
        { return _M_str[0]; }

        constexpr char& back() noexcept
        { return _M_str[_M_len - 1]; }

        /* Data */

        constexpr char* data() noexcept { return _M_str; }
        constexpr const char* data() const noexcept { return _M_str; }

        constexpr const char* c_str() const noexcept
        { return _M_str ? _M_str : ""; }

        /* Modifiers */

        constexpr void clear() noexcept
        {
            _M_len = 0;
            if (_M_str)
                _M_str[0] = '\0';
        }

        constexpr void push_back(char c) noexcept
        {
            _M_str[_M_len++] = c;
            _M_str[_M_len] = '\0';
        }

        constexpr void pop_back() noexcept
        {
            --_M_len;
            _M_str[_M_len] = '\0';
        }

        constexpr void append(const char* s) noexcept
        {
            while (*s) {
                _M_str[_M_len++] = *s++;
            }
            _M_str[_M_len] = '\0';
        }

        constexpr void append(const char* s, size_type n) noexcept
        {
            for (size_type i = 0; i < n; ++i)
                _M_str[_M_len++] = s[i];
            _M_str[_M_len] = '\0';
        }

        constexpr void append(string_view sv) noexcept
        {
            append(sv.data(), sv.size());
        }

        constexpr void assign(const char* s) noexcept
        {
            _M_len = 0;
            append(s);
        }

        constexpr void assign(const char* s, size_type n) noexcept
        {
            _M_len = 0;
            append(s, n);
        }

        constexpr void resize(size_type n) noexcept
        {
            _M_len = n;
            _M_str[_M_len] = '\0';
        }

        constexpr void swap(string& other) noexcept
        {
            auto str = this->_M_str;
            auto len = this->_M_len;
            auto cap = this->_M_cap;
            
            this->_M_str = other._M_str; 
            this->_M_len = other._M_len;
            this->_M_cap = other._M_cap;

            other._M_str = str;
            other._M_len = len;
            other._M_cap = cap;
        }

        /* Conversions */

        constexpr operator string_view() const noexcept
        {
            return string_view{_M_str, _M_len};
        }

    private:
        char*     _M_str;
        size_type _M_len;
        size_type _M_cap;
    };

    /*
     * Convenience: fixed-size string with internal storage
     */
    template<unsigned N>
    class static_string : public string {
    public:
        constexpr static_string() noexcept
            : string(_buffer, N)
        {}

        constexpr static_string(const char* s) noexcept
            : string(_buffer, N, s)
        {}
    };

} // namespace std

#endif /* STRING_HPP */
