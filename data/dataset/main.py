import csv

with open('history_filtered.csv', 'w', newline='') as out_f:
    writer = csv.writer(out_f)

    with open('history.csv', newline='') as in_f:
        reader = csv.reader(in_f)
        # Transfer header
        writer.writerow(next(reader))

        for row in reader:
            if any("fapello" in s.lower() for s in row):
                continue  # skip row / don't write

            writer.writerow(row)