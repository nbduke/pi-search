#include <stdlib.h>
#include <stdio.h>

#include <gmp.h>
#include <mpfr.h>

int main (int argc, char** argv)
{
	int prec, base;
	mpfr_t pi;
	
	if (argc != 3)
	{
		printf("./pi_gen <base> <precision>\n");
		return -1;
	}

	base = strtol(argv[1], 0, 10);
	prec = strtol(argv[2], 0, 10);
	
	mpfr_init2(pi, prec);
	mpfr_const_pi(pi, MPFR_RNDD);

	mpfr_out_str(stdout, base, 0, pi, MPFR_RNDD);
	putchar('\n');

	mpfr_clear(pi);
	mpfr_free_cache();

	return 0;
}