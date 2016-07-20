#pragma once
#include <libiptc/libiptc.h>

#if defined(_MSC_VER)
//  Microsoft 
#define EXPORT __declspec(dllexport)
#define IMPORT __declspec(dllimport)
#elif defined(_GCC)
//  GCC
#define EXPORT __attribute__((visibility("default")))
#define IMPORT
#else
//  do nothing and hope for the best?
#define EXPORT
#define IMPORT
#pragma warning Unknown dynamic link import/export semantics.
#endif

#ifdef __cplusplus
extern "C" {
#endif
	extern EXPORT const char* output_rule4(const struct ipt_entry *e, void *h, const char *chain, int counters);
	extern EXPORT int execute_command(const char* rule, void *h);
	extern EXPORT int init_helper(void);
	extern EXPORT void* init_handle(const char* table);
	extern EXPORT char* ipth_bpf_compile(const char* dlt, int length);
#ifdef __cplusplus
}
#endif
