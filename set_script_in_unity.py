import os

def set_script(filepath: str):
    with open(filepath, 'r') as f:
        content = f.read()

    original_lines = content.split('\n')
    original_lines = [line for line in original_lines if line != '' and line != '\n']
    lines = ['using UnityEngine;']
    for line in original_lines:
        strip_line = line.strip()
        if '[SerializeField]' in strip_line:
            lines.append(line)
            continue

        if strip_line.startswith('namespace'):
            lines.append(line)
            continue

        if strip_line.startswith('public class'):
            lines.append(line)
            continue

    lines.append('    }')
    lines.append('}')

    basename = os.path.basename(filepath)
    new_content = '\n'.join(lines)
    #with open('../../../Unity/OWWhole/RetryExport/ExportedProject/Assets/orclecledll/' + basename, 'w') as f:
    #    f.write(new_content)
    #with open('../../../Unity/OWPickUpChert/Assets/orclecledll/' + basename, 'w') as f:
    #    f.write(new_content)
    with open('DummyForUnity/' + basename, 'w') as f:
        f.write(new_content)

def main():
    set_script('PickUpChert/PathProbe.cs')

if __name__ == '__main__':
    main()
