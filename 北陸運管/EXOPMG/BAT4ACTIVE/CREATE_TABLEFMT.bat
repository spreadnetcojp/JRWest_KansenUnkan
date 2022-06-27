FOR /F %%a IN (DATA_LIST.txt) DO (
   BCP %2.dbo.%%a format nul -S %1 -U %3 -P %4 -n -f .\FMT\%%a.fmt
)
