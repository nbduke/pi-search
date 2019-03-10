#include <stdio.h>

#include <gmp.h>
#include <mpfr.h>

int main (void)
{
  int prec;
  unsigned int i;
  mpfr_t s, t, u;

  prec = 500;
  mpfr_init2 (t, prec);
  mpfr_set_d (t, 1.0, MPFR_RNDD);
  mpfr_init2 (s, prec);
  mpfr_set_d (s, 1.0, MPFR_RNDD);
  mpfr_init2 (u, prec);
  for (i = 1; i <= 100; i++)
    {
      mpfr_mul_ui (t, t, i, MPFR_RNDU);
      mpfr_set_d (u, 1.0, MPFR_RNDD);
      mpfr_div (u, u, t, MPFR_RNDD);
      mpfr_add (s, s, u, MPFR_RNDD);
    }
  printf ("Sum is ");
  mpfr_out_str (stdout, 26, 0, s, MPFR_RNDD);
  putchar ('\n');
  mpfr_clear (s);
  mpfr_clear (t);
  mpfr_clear (u);
  mpfr_free_cache ();
  return 0;
}