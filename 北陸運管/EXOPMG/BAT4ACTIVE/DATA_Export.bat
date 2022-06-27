FOR /F %%a IN (DATA_LIST.txt) DO (
   BCP "SELECT * FROM %2.dbo.%%a" queryout .\DATA\%%a.dat -S %1 -U %3 -P %4 -f FMT\%%a.fmt
)
