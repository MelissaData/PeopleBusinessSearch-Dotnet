#!/bin/bash

# Builds and runs the Melissa People Business Search Cloud API .NET sample.
#
# This script builds PeopleBusinessSearchDotnet with dotnet publish, then runs the resulting
# executable, passing along the license and (if supplied) the search fields.
#
# Overall flow:
#   1. Parse the command-line options below.
#   2. Resolve the license (--license, then a prompt, then the MD_LICENSE environment variable).
#   3. Publish PeopleBusinessSearchDotnet in Release configuration to ./PeopleBusinessSearchDotnet/Build.
#   4. Run the built executable: one-shot mode if any search field was supplied,
#      otherwise interactive mode (the .NET program prompts for each field).
#
# Options (each takes a value):
#   --maxrecords           Maximum number of records to return.
#   --matchlevel           Match level to use for the search.
#   --addressline1         Street address to search.
#   --locality             City/locality to search.
#   --administrativearea   State/administrative area to search.
#   --postal               Postal code to search.
#   --anyname              Person or business name to search.
#   --license              License string. If omitted, the script prompts for it; if the prompt
#                          is left blank, it falls back to MD_LICENSE. Running without --license
#                          always prompts, even when MD_LICENSE is set.
#
# Paths are relative to the current directory, so run the script from its own folder.
#
# Examples:
#   ./PeopleBusinessSearchDotnet.sh --license "your-license"
#   ./PeopleBusinessSearchDotnet.sh --maxrecords "10" --matchlevel "10" --addressline1 "22382 Avenida Empresa" --locality "Rancho Santa Margarita" --administrativearea "CA" --postal "92688" --anyname "Melissa Data" --license "your-license"

######################### Constants ##########################

RED='\033[0;31m' #RED
NC='\033[0m' # No Color

######################### Parameters ##########################

maxrecords=""
matchlevel=""
addressline1=""
locality=""
administrativearea=""
postal=""
anyname=""
license=""

# Read each --flag and its value. A flag with no value, or whose value starts with "-"
# (such as another option), is an error. Unrecognized options are ignored.
while [ $# -gt 0 ] ; do
  case $1 in
    --maxrecords)  
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'maxrecords\'.${NC}\n"  
            exit 1
        fi 

        maxrecords="$2"
        shift
        ;;

    --matchlevel)  
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'matchlevel\'.${NC}\n"  
            exit 1
        fi 

        matchlevel="$2"
        shift
        ;;

    --addressline1) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'addressline1\'.${NC}\n"  
            exit 1
        fi 

        addressline1="$2"
        shift
        ;;
    --locality)  
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'locality\'.${NC}\n"  
            exit 1
        fi 

        locality="$2"
        shift
        ;;
    --administrativearea) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'administrativearea\'.${NC}\n"  
            exit 1
        fi 

        administrativearea="$2"
        shift
        ;;
    --postal)         
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'postal\'.${NC}\n"  
            exit 1
        fi 
        
        postal="$2"
        shift
        ;;
    --anyname) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'anyname\'.${NC}\n"  
            exit 1
        fi 

        anyname="$2"
        shift
        ;;
    --license) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'license\'.${NC}\n"  
            exit 1
        fi 

        license="$2"
        shift 
        ;;
  esac
  shift
done

# Build paths are relative to the current directory (not the script's location)
CurrentPath="$(pwd)"
ProjectPath="$CurrentPath/PeopleBusinessSearchDotnet"
BuildPath="$ProjectPath/Build"

if [ ! -d "$BuildPath" ];
then
    mkdir "$BuildPath"
fi

########################## Main ############################
printf "\n================ Melissa People Business Search Cloud API ==================\n"

# Get license (either from parameters or user input)
if [ -z "$license" ];
then
  printf "Please enter your license string: "
  read license
fi

# Check for License from Environment Variables 
if [ -z "$license" ];
then
  license=`echo $MD_LICENSE` 
fi

if [ -z "$license" ];
then
  printf "\nLicense String is invalid!\n"
  exit 1
fi

# Start program
# Build project
printf "\n=============================== BUILD PROJECT ==============================\n"

dotnet publish -f="net7.0" -c Release -o "$BuildPath" PeopleBusinessSearchDotnet/PeopleBusinessSearchDotnet.csproj

# Run project
# No search fields supplied -> run interactively; otherwise pass them through for one-shot mode.
# Bash passes empty quoted values as real empty arguments, so unsupplied fields arrive
# empty and the program prompts for them.
if [ -z "$maxrecords" ] && [ -z "$matchlevel" ] && [ -z "$addressline1" ] && [ -z "$locality" ] && [ -z "$administrativearea" ] && [ -z "$postal" ] && [ -z "$anyname" ];
then
    dotnet "$BuildPath"/PeopleBusinessSearchDotnet.dll --license "$license"
else
    dotnet "$BuildPath"/PeopleBusinessSearchDotnet.dll --license "$license" --maxrecords "$maxrecords" --matchlevel "$matchlevel" --addressline1 "$addressline1" --locality "$locality" --administrativearea "$administrativearea" --postal "$postal" --anyname "$anyname"
fi


