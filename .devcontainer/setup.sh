echo "Running setup.sh..."

dotnet tool restore
dotnet restore IntroMutationTesting.sln

echo "setup.sh complete."
