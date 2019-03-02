#    Each line (splittet): ('line1', '::', 'value')
#                 ^1^     ^2^     ^3^
#    1 = Name    2 = Operator/Definer    3 = Value
#
#    Comment line (splittet): ('@ comment')
#    Check if the first char is ^ then we can conclude it's a comment
#
#    But first make sure the first char in the list is '[', then find
#    the closing (']') and proceed!

def load (path):
    # Get the file contents and make it a list
    content = list(open(path, 'r'))

    # Loop through every line in the contents list
    for i, element in enumerate(content):
        # Replace new lines and tabs with nothing
        content[i] = content[i].replace('\n', '')
        content[i] = content[i].replace('\t', '')

    # Loop through all the lines again
    for ist, element in enumerate(content):
        # Check if the current line is an open bracket
        if content[ist] == '[':
            # If so, loop through the lines again. But this time so it will
            for ien, element in enumerate(content):
                # Check if the current line is an close bracket
                if content[ien] == '];':
                    # If so, check the text inside it

                    # Create variables
                    temp = ''
                    insides = []
                    elements = []
                    results = []

                    # Get all lines between the open and close bracket
                    for i, element in enumerate(content[ist : ien]):
                        # Set the temp variable to the current line, but with replace spaces
                        temp = content[i].replace(' ', '')

                        # Check if the current line/temp starts with @
                        if temp.startswith('@'):
                            # Ignore it/go to next line
                            continue

                        # Check if the current line/temp starts with /////
                        elif temp.startswith('/////'):
                            continue

                        # Check if the current line/temp starts with [
                        elif temp.startswith('['):
                            # Ignore it/go to next line
                            continue

                        # Check if the current line/temp starts with ]
                        elif temp.startswith(']'):
                            # Ignore it/go to next line
                            continue

                        # Check if the current line/temp is empty
                        elif temp == '':
                            # Ignore it/go to next line
                            continue

                        # If the current line does not match any above,
                        # then add it to the insides list
                        insides.append(content[i])

                    # Loop through all lines in the insides list
                    for i, line in enumerate(insides):
                        # Set the temp variable to the current line, but with replace spaces
                        temp = insides[i].replace(' ', '')

                        # Check if the line starts with a ?
                        if temp.startswith('?'):    # This is a variable
                            # Replace ' and ? in the line to nothing
                            line = line.replace("'", '')
                            line = line.replace('?', '')

                            # Split the line into words and symbols
                            elements = line.split()

                            # Check if they're too many args
                            if len(elements[2:]) > 2:
                                # If so, print a message
                                print('Too many args')

                            try:
                                # Try and set the result message
                                res = (' '.join(elements[2:]), elements[1], elements[0])
                            except:
                                # Line is empty
                                continue

                            # Add the result text to the results list
                            results.append(res)

                        # Check if the line starts with a '
                        elif temp.startswith("'"):   # This is a normal text var
                            # Replace ' in the line to nothing
                            line = line.replace("'", '')

                            # Split the line into words and symbols
                            elements = line.split()

                            # Check if they're too many args
                            if len(elements[2:]) > 2:
                                # If so, print a message
                                print('Too many args')

                            try:
                                # Try and set the result message
                                res = ("'" + ' '.join(elements[2:]) + "'", elements[1], elements[0])
                            except:
                                # Line is empty
                                continue

                            # Add the result text to the results list
                            results.append(res)

                    # Make the results list to a dictionary, so we can get items by name
                    results = dict([i[:: - 2] for i in results])

                    #print(results)
                    return results

def read (path):
    content = list(open(path, 'r'))

    for i, element in enumerate(content):
        # Replace new lines and tabs with nothing
        content[i] = content[i].replace('\n', '')
        content[i] = content[i].replace('\t', '')

    for ist, element in enumerate(content):
        # Check if the current line is an open bracket
        if content[ist] == '[':
            # If so, loop through the lines again. But this time so it will
            for ien, element in enumerate(content):
                # Check if the current line is an close bracket
                if content[ien] == '];':
                    # If so, check the text inside it

                    # Create variables
                    temp = ''
                    insides = []
                    elements = []
                    results = []

                    # Get all lines between the open and close bracket
                    for i, element in enumerate(content[ist : ien]):
                        # Set the temp variable to the current line, but with replace spaces
                        temp = content[i].replace(' ', '')

                        # Check if the current line/temp starts with @
                        if temp.startswith('@'):
                            # Ignore it/go to next line
                            continue

                        # Check if the current line/temp starts with /////
                        elif temp.startswith('/////'):
                            continue

                        # Check if the current line/temp starts with [
                        elif temp.startswith('['):
                            # Ignore it/go to next line
                            continue

                        # Check if the current line/temp starts with ]
                        elif temp.startswith(']'):
                            # Ignore it/go to next line
                            continue

                        # Check if the current line/temp is empty
                        elif temp == '':
                            # Ignore it/go to next line
                            continue

                        # If the current line does not match any above,
                        # then add it to the insides list
                        insides.append(content[i])

                    # Loop through all lines in the insides list
                    for i, line in enumerate(insides):
                        # Set the temp variable to the current line, but with replace spaces
                        temp = insides[i].replace(' ', '')

                        # Check if the line starts with a ?
                        if temp.startswith('?'):    # This is a variable
                            # Replace ' and ? in the line to nothing
                            line = line.replace("'", '')
                            line = line.replace('?', '')

                            # Split the line into words and symbols
                            elements = line.split()

                            # Check if they're too many args
                            if len(elements[2:]) > 2:
                                # If so, print a message
                                print('Too many args')

                            try:
                                # Try and set the result message
                                res = (' '.join(elements[2:]), elements[1], elements[0])
                            except:
                                # Line is empty
                                continue

                            # Add the result text to the results list
                            results.append(res)

                        # Check if the line starts with a '
                        elif temp.startswith("'"):   # This is a normal text var
                            # Replace ' in the line to nothing
                            line = line.replace("'", '')

                            # Split the line into words and symbols
                            elements = line.split()

                            # Check if they're too many args
                            if len(elements[2:]) > 2:
                                # If so, print a message
                                print('Too many args')

                            try:
                                # Try and set the result message
                                res = ("'" + ' '.join(elements[2:]) + "'", elements[1], elements[0])
                            except:
                                # Line is empty
                                continue

                            # Add the result text to the results list
                            results.append(res)

                    return results

def save (content, path):
    before = read(path)

    parents = list(content.keys())
    values = list(content.values())

    #for i, item in enumerate(parents):
        #parents[i] = "'{0}'".format(parents[i])

    #for i, item in enumerate(values):
        #values[i] = "'{0}'".format(values[i])

    for ib, line in enumerate(before):
        for ip, item in enumerate(parents):
            if line[-1] == item:
                if values[ip] != line[0]:
                    blist = [list(elem) for elem in before]
                    blist[ib][0] = "'{}'".format(values[ip])
                    before = [tuple(elem) for elem in blist]

    file = open(path, 'r')
    content = file.readlines()
    file.close()

    temp = ''

    for line in content:
        temp = line.replace(' ', '')
        temp = line.replace("'", '')
        temp = line.replace('\n', '')

        words = temp.split()
        print(words)

        #if words[0] == 'Name':
            #print('Yes?')
    #print(before)

res = load('../cnt.eve')
#print(res[0])
res['Name'] = 'Martin'
res['Age'] = 22
save(res, '../cnt.eve')
