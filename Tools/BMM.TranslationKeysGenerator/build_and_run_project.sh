build_project_path="$1/build_project.sh"
run_program_path="$1/run_program.sh"
translations_file_path="$3/$4"

./$build_project_path $1
./$run_program_path -s $3 -f $translations_file_path