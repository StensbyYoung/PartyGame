import os
import sys
import argparse

os.chdir("..")
root_path = os.getcwd()
build_path = root_path + "\\TestGame"
win_path = root_path + "\\TestGame\\bin\\Release\\net6.0\\win-x64"
linux_path = root_path + "\\TestGame\\bin\\Release\\net6.0\\linux-arm64"

parser = argparse.ArgumentParser(description="gamebuilder script")
parser.add_argument("-c", "--clean", action="store_true", help="clean specified build directory")
parser.add_argument("target", nargs="+", help="target platform")
args = parser.parse_args()

config = vars(args)

for p in args.target:
    if not args.clean:
        os.chdir(root_path)
        if p == "win" or p == "windows":
            print("Building for windows")
            os.chdir(build_path)
            os.system("dotnet publish --sc -r win-x64 -c Release")
        elif p == "linux":
            print("Building for linux")
            os.chdir(build_path)
            os.system("dotnet publish --sc -r linux-arm64 -c Release")
        else:
            print("%s is not a valid platform" % p)
    elif args.clean:
        if p == "win" or p == "windows":
            print("Cleaning build path for windows")
            os.chdir(win_path)
            print(os.getcwd())
            os.system("rm -r publish")
        elif p == "linux":
            print("Cleaning build path for linux")
            os.chdir(linux_path)
            os.system("rm -r publish") 
