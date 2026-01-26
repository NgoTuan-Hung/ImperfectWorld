import os

def count_cs_loc(root_dir="."):
    total_files = 0
    total_lines = 0

    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file.lower().endswith(".cs"):
                total_files += 1
                file_path = os.path.join(root, file)
                try:
                    with open(file_path, "r", encoding="utf-8", errors="ignore") as f:
                        lines = f.readlines()
                        total_lines += len(lines)
                except Exception as e:
                    print(f"Could not read {file_path}: {e}")

    return total_files, total_lines

if __name__ == "__main__":
    files, lines = count_cs_loc()
    print(f"Total .cs files: {files}")
    print(f"Total lines of code: {lines}")
