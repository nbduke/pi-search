#include <math.h>
#include <stdlib.h>
#include <stdio.h>

#include <gmp.h>
#include <mpfr.h>

int main (int argc, char** argv)
{
	double base, digits, prec;
	FILE* output;
	size_t digits_written;
	mpfr_t pi;
	
	if (argc != 3)
	{
		printf("./pi_gen <base> <digits>\n");
		return -1;
	}

	base = strtod(argv[1], 0);
	digits = strtod(argv[2], 0);
	prec = ceil(log2(base) * digits) - 1;
	output = fopen("pi.txt", "w");
	
	mpfr_init2(pi, (size_t)prec);
	mpfr_const_pi(pi, MPFR_RNDD);

	digits_written = mpfr_out_str(
		output,
		(int)base,
		(size_t)digits,
		pi,
		MPFR_RNDD
	);
	
	if (digits_written == 0)
	{
		printf("An unknown error occurred.\n");
	}
	else
	{
		printf("Digits written: %lu\n", digits_written - 1);
	}

	mpfr_clear(pi);
	mpfr_free_cache();

	return 0;
}