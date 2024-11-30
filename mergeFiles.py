import os

def collect_files_to_merge(directories, output_file_name):
    # Получаем путь к директории, где находится скрипт
    script_directory = os.path.dirname(os.path.abspath(__file__))
    output_file_path = os.path.join(script_directory, output_file_name)

    with open(output_file_path, 'w', encoding='utf-8') as outfile:
        for directory in directories:
            if os.path.isfile(directory):
                # Если это конкретный файл, считываем его содержимое
                with open(directory, 'r', encoding='utf-8') as infile:
                    outfile.write(infile.read() + '\n')
            else:
                # Если это папка, проходим по всем файлам в папке
                for root, _, files in os.walk(directory):
                    for file in files:
                        file_path = os.path.join(root, file)
                        if file.endswith('.txt') or file.endswith('.cs'):  # Фильтр для файлов
                            with open(file_path, 'r', encoding='utf-8') as infile:
                                outfile.write(infile.read() + '\n')
    return output_file_path

# Указание директорий и файлов
directories_to_merge = [
    r"C:\Users\davtian\Downloads\Telegram Desktop\RoomReservationSystem_V4 (2)\RoomReservationSystem\new_sqript.txt",
    r"C:\Users\davtian\Downloads\Telegram Desktop\RoomReservationSystem_V4 (2)\RoomReservationSystem\Services",
    r"C:\Users\davtian\Downloads\Telegram Desktop\RoomReservationSystem_V4 (2)\RoomReservationSystem\Models",
    r"C:\Users\davtian\Downloads\Telegram Desktop\RoomReservationSystem_V4 (2)\RoomReservationSystem\Models\Auth",
    r"C:\Users\davtian\Downloads\Telegram Desktop\RoomReservationSystem_V4 (2)\RoomReservationSystem\Controllers",
    r"C:\Users\davtian\Downloads\Telegram Desktop\RoomReservationSystem_V4 (2)\RoomReservationSystem\Controllers\Admin",
    r"C:\Users\davtian\Downloads\Telegram Desktop\RoomReservationSystem_V4 (2)\RoomReservationSystem\Data"
]

# Указание имени итогового файла
output_file_name = "merged_output.txt"

# Запуск функции
output_file_path = collect_files_to_merge(directories_to_merge, output_file_name)

print(f"Объединение завершено! Результат сохранён в {output_file_path}")
