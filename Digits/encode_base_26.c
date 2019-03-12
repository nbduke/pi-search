#include <stdio.h>

int main(int argc, char** argv)
{
	FILE *input, *output;
	char c;
	int alphabetIndex;
	
	if (argc != 2)
	{
		printf("encode_base_26 <input_file>");
		return -1;
	}

	input = fopen(argv[1], "r");
	output = fopen("pi_alphabet.txt", "w");

	while (!(feof(input) || ferror(input)))
	{
		fscanf(input, "%c", &c);
		if ('0' <= c && c <= '9')
		{
			alphabetIndex = (c - '0');
		}
		else if ('a' <= c && c <= 'z')
		{
			alphabetIndex = (c - 'a') + 10;
		}
		else
		{
			continue;
		}
		fprintf(output, "%c", 'a' + alphabetIndex);
	}
	
	fclose(input);
	fclose(output);
	return 0;
}