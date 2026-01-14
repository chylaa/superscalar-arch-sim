#ifndef STRING_VIEW_HPP
#define STRING_VIEW_HPP

#include "simlib.hpp"

namespace std {

    class string_view {
        static constexpr unsigned npos = unsigned(-1);

    public:
        constexpr
        string_view() : _M_len{0}, _M_str{nullptr} 
        {}

        constexpr
        explicit string_view(const char* _s) : _M_str{_s}, _M_len{strlen(_s)}
        {}

        constexpr
        string_view(const char* _s, unsigned _n) : _M_str{_s}, _M_len{_n}
        {}

        string_view(string_view&&) = default;
        string_view &operator=(string_view&&) = default;

        string_view(const string_view&) = default;
        string_view &operator=(const string_view&) = default;

        ~string_view() = default;

        constexpr const char*
        begin() const
        { return this->_M_str; }
  
        constexpr const char*
        end() const
        { return this->_M_str + this->_M_len; }
  
        constexpr const char*
        cbegin() const
        { return this->_M_str; }
  
        constexpr const char*
        cend() const
        { return this->_M_str + this->_M_len; }

        constexpr unsigned
        size() const
        { return this->_M_len; }
  
        constexpr unsigned
        length() const
        { return _M_len; }

        constexpr bool
        empty() const
        { return this->_M_len == 0; }

        constexpr unsigned
        max_size() const
        { return (npos - sizeof(unsigned) - sizeof(void*)) / sizeof(char) / 4; }
  
        constexpr const char&
        operator[](unsigned __pos) const
        { return *(this->_M_str + __pos); }

        constexpr const char&
        at(unsigned __pos) const
        { return *(this->_M_str + __pos); }

        constexpr const char&
        front() const noexcept
        { return *this->_M_str; }
  
        constexpr const char&
        back() const noexcept
        { return *(this->_M_str + this->_M_len - 1); }
  
        constexpr const char*
        data() const noexcept
        { return this->_M_str; }
  
        constexpr void
        remove_prefix(unsigned __n)
        {
            this->_M_str += __n;
            this->_M_len -= __n;
        }
  
        constexpr void
        remove_suffix(unsigned __n)
        { this->_M_len -= __n; }

        constexpr void
        swap(string_view& __sv)
        {
            auto __tmp = *this;
            *this = __sv;
            __sv = __tmp;
        }
    
        constexpr string_view
        substr(unsigned __pos = 0, unsigned __n = npos) const
        {
            const unsigned __rlen = min<unsigned>(__n, _M_len - __pos);
            return string_view{_M_str + __pos, __rlen};
        }
  
    private:
        unsigned     _M_len;
        const char*  _M_str;
    };
}

#endif /* STRING_VIEW_HPP */